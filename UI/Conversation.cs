using System.Threading;
using DialogueSystem.Core;
using DialogueSystem.Engine;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace DialogueSystem.UI;

public class Conversation
{
    public Graph graph { get; private set; }
    private QuestionNode currNode;
    public bool convoStarted { get; private set; }
    public UIMenu convoMenu;
    private List<QuestionNode> questionPool;
    public event EventHandler<(QuestionNode, AnswerNode)> OnQuestionSelect;
    public event EventHandler OnCoversationEnded;
    private GameFiber onItemSelectFiber;
    private GameFiber conditionCheckFiber;
    internal Dictionary<string, bool> conditionPool;
    private CancellationTokenSource conditionCancellationTokenSource;


    public Conversation(Graph graph, UIMenu convoMenu, List<QuestionNode> startNodes)
    {
        this.graph = graph;
        currNode = null;
        convoStarted = false;
        this.convoMenu = convoMenu;
        questionPool = new List<QuestionNode>();
        questionPool.AddRange(startNodes);
        conditionPool = new Dictionary<string, bool>();
    }

    /// <summary>
    /// This method initializes the conversation by setting the starting edges and cloning the adjacency list.
    /// Also, this method adds the initial items to the RNUI menu.
    /// This method has to be called before Run(). Else, the conversation will not work.
    /// </summary>
    public void Init()
    {
        if (convoStarted) return;
        graph.startingEdges = new List<Edge>(graph.edges);
        graph.CloneAdjList();
        UpdateMenu(true);
    }

    private void UpdateMenu(bool start = false)
    {
        convoMenu.Clear();
        if (!start)
        {
            var list = graph.GetConnectedNodes(currNode);
            Game.LogTrivial(
                $"Adding all nodes({list.Count}) connected to {currNode.value} to the questionPool");
            questionPool.Clear();
            if (currNode != null && !currNode.removeQuestionAfterAsked)
            {
                Game.LogTrivial(
                    $"Adding {currNode.value} back due to removeQuestionAfterAsked being false");
                questionPool.Add(currNode);
            }
            questionPool.AddRange(list);
        }

        foreach (var item in questionPool)
        {
            Game.LogTrivial($"Adding {item.value} to the menu");
            convoMenu.AddItem(new UIMenuItem(item.value));
        }
    }

    /// <summary>
    /// The converstion is active. This method will subscribe to the OnItemSelect event of the RNUI menu.
    /// If your plugin is using the OnItemSelect for the menu that the conversation uses, the dialogue system will not work.
    /// </summary>
    public void Run()
    {
        convoMenu.OnItemSelect += ItemSelectWarapper;
        convoMenu.OnMenuOpen += StartCheckingConditions;
        convoMenu.OnMenuClose += StopCheckingConditions;
        Game.LogTrivial("Subbing to events");
    }

    private void StartCheckingConditions(UIMenu sender)
    {
        if (conditionCheckFiber != null && conditionCheckFiber.IsAlive)
        {
            Game.LogTrivial("Condition checking fiber already running.");
            return;
        }
        conditionCancellationTokenSource = new CancellationTokenSource();
        CancellationTokenManager.RegisterSource(conditionCancellationTokenSource);
        CancellationToken cancellationToken = conditionCancellationTokenSource.Token;

        conditionCheckFiber = GameFiber.StartNew(() =>
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    CheckConditions();
                    GameFiber.Yield();
                    GameFiber.Sleep(1500);
                }
            }
            catch (ThreadAbortException)
            {
                Game.LogTrivial("Condition check fiber aborted (unexpectedly).");
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"Error in condition check fiber: {ex}");
            }
            finally
            {
                Game.LogTrivial("Condition check fiber stopped.");
                conditionCancellationTokenSource?.Dispose();
                conditionCancellationTokenSource = null;
            }
        });
        Game.LogTrivial("Condition checking fiber started.");
    }

    private void StopCheckingConditions(UIMenu sender)
    {
        if (conditionCancellationTokenSource != null &&
            !conditionCancellationTokenSource.IsCancellationRequested)
        {
            Game.LogTrivial("Requesting condition check fiber to stop...");
            conditionCancellationTokenSource.Cancel();
            conditionPool.Clear();
        }
        else if (conditionCheckFiber == null || !conditionCheckFiber.IsAlive)
        {
            Game.LogTrivial("Condition checking fiber is not running or already stopped.");
        }
    }

    private void CheckConditions()
    {
        foreach (var qNode in questionPool)
        {
            foreach (var aNode in qNode.possibleAnswers)
            {
                if (aNode.condition != null)
                {
                    if (conditionPool.ContainsKey(aNode.ID))
                    {
                        conditionPool[aNode.ID] = aNode.condition.Invoke();
                    }
                    else
                    {
                        conditionPool.Add(aNode.ID, aNode.condition.Invoke());
                    }
                }
            }
        }
    }

    private void ItemSelectWarapper(UIMenu uiMenu, UIMenuItem selectedItem, int index)
    {
        if (onItemSelectFiber == null || !onItemSelectFiber.IsAlive)
        {
            onItemSelectFiber = GameFiber.StartNew(delegate
            {
                OnItemSelect(uiMenu, selectedItem, index);
            });
        }
        else
        {
            Game.LogTrivial("Item selection ignored: Another fiber is already processing.");
        }
    }

    private void OnItemSelect(UIMenu uiMenu, UIMenuItem selectedItem, int index)
    {
        Game.LogTrivial("In Dialogue System item select");
        AnswerNode answer = null;
        QuestionNode qNode = questionPool[index];
        currNode = qNode;
        Game.DisplaySubtitle(qNode.value);
        answer = qNode.ChooseQuestion(graph, this);
        Game.LogTrivial($"Question chosen: {qNode.value}");
        Game.LogTrivial($"Answer chosen: {answer.value}");
        OnQuestionSelect?.Invoke(this, (qNode, answer));
        Game.DisplaySubtitle(answer.value);
        if (answer.action != null) answer.action.Invoke();
        if (answer.endsConversation)
        {
            EndConvo();
            return;
        }

        UpdateMenu();
        if (questionPool.Count == 0)
        {
            EndConvo();
        }

        convoStarted = true;
    }

    private void EndConvo()
    {
        Game.LogTrivial("Ending Conversation");
        convoMenu.Close();
        foreach (QuestionNode q in graph.nodes)
        {
            q.ResetChosenAnswer();
        }

        graph.edges = graph.startingEdges;
        graph.adjList = graph.startingAdjList;
        convoMenu.OnItemSelect -= OnItemSelect;
        convoMenu.OnMenuOpen -= StartCheckingConditions;
        convoMenu.OnMenuClose -= StopCheckingConditions;
        convoStarted = false;
        currNode = null;
        OnCoversationEnded?.Invoke(this, EventArgs.Empty);
    }
}
