using System.IO;
using DialogueSystem.Core;
using DialogueSystem.Core.Logic;
using DialogueSystem.UI;
using Newtonsoft.Json.Linq;
using Rage;
using RAGENativeUI;

namespace DialogueSystem.Engine;

internal class DialogueLoader
{
    internal static Graph ParseGraphManually(string jsonContent)
    {
        var nodes = new List<QuestionNode>();
        var edges = new List<Edge>();
        var sNodes = new List<QuestionNode>();


        JObject rootJObject = JObject.Parse(jsonContent);

        // --- Phase 1: Parse Nodes and build a lookup dictionary ---
        JArray nodesJsonArray = (JArray)rootJObject["nodes"];
        Dictionary<string, QuestionNode> questionLookup = new Dictionary<string, QuestionNode>();
        Dictionary<string, AnswerNode> answerLookup = new Dictionary<string, AnswerNode>();

        if (nodesJsonArray != null)
        {
            foreach (JToken nodeToken in nodesJsonArray)
            {
                // Decision logic for polymorphic nodes
                QuestionNode node;
                JToken dataToken = nodeToken["data"];
                JToken answersToken = dataToken?["answers"];

                string id = (string)nodeToken["id"];
                string qText = (string)dataToken["questionText"];
                bool removeQuestionAfterAsked = (bool)nodeToken["removeQuestionAfterAsked"];
                bool startsConversation = (bool)nodeToken["startsConversation"];


                if (answersToken != null && answersToken.Type == JTokenType.Array)
                {
                    // It's a QuestionNode if it has an answers array in data
                    QuestionNode questionNode = new QuestionNode();
                    questionNode.possibleAnswers =
                        new List<AnswerNode>(); // Initialize answers list

                    // Populate answers
                    foreach (JToken answerToken in answersToken)
                    {
                        ExternalCondition con = null;
                        string conditionString = (string)answerToken["condition"];
                        if (conditionString != null && !string.IsNullOrEmpty(conditionString))
                        {
                            con = new ExternalCondition(conditionString);
                        }

                        ExternalAction act = null;
                        string actionString = (string)answerToken["action"];
                        if (actionString != null && !string.IsNullOrEmpty(actionString))
                        {
                            act = new ExternalAction(actionString);
                        }

                        var aNode = new AnswerNode()
                        {
                            ID = (string)answerToken["id"],
                            value = (string)answerToken["text"],
                            probability =
                                (int)answerToken["probability"],
                            condition = con,
                            endsConversation = (bool?)answerToken["endsCondition"] ?? false,
                            action = act
                        };
                        questionNode.possibleAnswers.Add(aNode);
                        answerLookup.Add((string)answerToken["id"], aNode);
                    }

                    node = questionNode;
                }
                else
                {
                    // If you had other node types, you'd add conditions here
                    // For now, if it's not a QuestionNode based on 'answers',
                    // we'll assume it's just a generic Node (if you had a concrete base)
                    // or throw an error if all must be QuestionNodes.
                    Console.WriteLine(
                        $"Warning: Node ID '{id}' does not appear to be a QuestionNode. Skipping specific data population.");
                    node =
                        new QuestionNode(); // Default to QuestionNode for now or make a GenericNode
                }

                // Populate common Node properties
                node.ID = id;
                node.value = qText;
                node.removeQuestionAfterAsked = removeQuestionAfterAsked;
                node.startsConversation = startsConversation;

                nodes.Add(node); // Add to the final list
                if (node.startsConversation)
                {
                    sNodes.Add(node);
                }

                if (questionLookup.ContainsKey(id)) // Add to lookup dictionary
                {
                    Console.WriteLine(
                        $"Error: Duplicate Node ID found while parsing: {node.ID}. Only the first instance will be used for connections.");
                    // You might want to skip adding the duplicate or handle it differently
                }

                questionLookup.Add(id, node);
            }
        }

        // --- Phase 2: Parse Connections (Edges) and wire them up ---
        JArray connectionsJsonArray = (JArray)rootJObject["connections"];

        if (connectionsJsonArray != null)
        {
            foreach (JToken connectionToken in connectionsJsonArray)
            {
                string edgeId = (string)connectionToken["id"];
                string fromNodeId = (string)connectionToken["from"]?["nodeId"];
                string toNodeId = (string)connectionToken["to"]?["nodeId"];

                QuestionNode fromNode = null;
                QuestionNode toNode = null;

                // Lookup 'From' node
                if (!string.IsNullOrEmpty(fromNodeId) &&
                    questionLookup.TryGetValue(fromNodeId, out QuestionNode foundFromNode))
                {
                    fromNode = foundFromNode;
                }
                else
                {
                    Console.WriteLine(
                        $"Warning: 'From' node with ID '{fromNodeId}' not found for edge '{edgeId}'.");
                }

                // Lookup 'To' node
                if (!string.IsNullOrEmpty(toNodeId) &&
                    questionLookup.TryGetValue(toNodeId, out QuestionNode foundToNode))
                {
                    toNode = foundToNode;
                }
                else
                {
                    Console.WriteLine(
                        $"Warning: 'To' node with ID '{toNodeId}' not found for edge '{edgeId}'.");
                }

                // Create and add the Edge object
                Game.LogTrivial($"adding edge from {fromNode.value} to {toNode.value}");
                edges.Add(new Edge(fromNode, toNode));
            }
        }

        JArray consequencesJsonArray = (JArray)rootJObject["consequences"];
        foreach (JToken entry in consequencesJsonArray)
        {
            JToken innerEntry = entry["consequences"];
            if (innerEntry != null && innerEntry.Type == JTokenType.Object)
            {
                var aNodeID = (string)innerEntry["answerNodeId"];
                AnswerNode answerNode = null;
                if (!string.IsNullOrEmpty(aNodeID) &&
                    answerLookup.TryGetValue(aNodeID, out AnswerNode a))
                {
                    answerNode = a;
                }
                else
                {
                    Console.WriteLine(
                        $"Warning: Answer Node {aNodeID}' not found.");
                    continue;
                }

                var questionsToAddArray = (JArray)innerEntry["questionsToAdd"];
                if (questionsToAddArray != null)
                {
                    foreach (JToken qIdToken in questionsToAddArray)
                    {
                        if (qIdToken.Type == JTokenType.String)
                        {
                            var idString = (string)qIdToken;
                            if (!string.IsNullOrEmpty(idString) &&
                                questionLookup.TryGetValue(idString, out QuestionNode question))
                            {
                                answerNode.questionsToAdd.Add(question);
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"Warning: Question Node {idString}' not found.");
                                continue;
                            }
                        }
                    }
                }
                
                var questionsToRemoveArray = (JArray)innerEntry["questionsToRemove"];
                if (questionsToRemoveArray != null)
                {
                    foreach (JToken qIdToken in questionsToRemoveArray)
                    {
                        if (qIdToken.Type == JTokenType.String)
                        {
                            var idString = (string)qIdToken;
                            if (!string.IsNullOrEmpty(idString) &&
                                questionLookup.TryGetValue(idString, out QuestionNode question))
                            {
                                answerNode.questionsToRemove.Add(question);
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"Warning: Question Node {idString}' not found.");
                                continue;
                            }
                        }
                    }
                }
            }
        }

        var result = new Graph(nodes, edges, new GraphConfig());
        result.nodesToStartConversation = sNodes;
        return result;
    }
}
