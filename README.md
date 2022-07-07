# card_game

# Client
When working on the client the most important thing to take note of it the structure to which is organized. The ENTIRE gameflow is server-authoritative, so it is the job of the client to effectivley communicate the information that is sent from the server.
The most important elements of the client are the NetworkManager class and the AnimationManager class. The Network manager makes sure we receive all the packages, connect to the server, and deal with all the networking issues. The AnimationManager is a simple queue which allows us to add Coroutines, (IEnumerator) functions which will allow us to organize the flow of our animations.

Essentially, the NetworkManager will receive all the messages, we write handlers, which will add what they want to do to the animation queue. This ensures that all the gameflow is straight forward.

## Sending Messages.
Sending messages from the client is incredibly easy to do, take for instance the end turn button.

```
(using RiptideNetworking.)
    public static void ChangeTurnMessge()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.end); // Creates the message and sends
        NetworkManager.Singleton.Client.Send(m);
    }
```
This will send a message immediatley to the server, however, if we wanted to delay this message, and send it after we've processed the other tasks on the client, we would use the AnimationManager like so.


```
(using RiptideNetworking.)
    public static void ChangeTurnMessge()
    {
      AnimationManager.Singleton.ADD_ANIMATION(HandleChangeTurn); // adds it to the managers priority queue
    }
    
    public static IEnumerator HandleChangeTurn()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServer.end); // Creates the message and sends
        NetworkManager.Singleton.Client.Send(m);
        yield break; // IEnumerator has to yield some value, or break
    }
```

## Receiving Messages
RiptideNetworking uses an easy method for keeping track of the incoming messages, using the message handler. As we don't want multiple animations starting at once as we receive multiple messages, we almost always use the AnimationManager. Take for instance this one which syncs the board after we perform any board operation.

```
    [MessageHandler((ushort) ServerToClient.boardUpdateEnemy)] // RECEIVE THIS KIND OF MESSAGE
    public static void BoardUpdateSyncOpponent(Message message)
    {
        Message m = System.ObjectExtensions.Copy(message); // WE HAVE THE COPY THE MESSAGE AS IT IS BY DEFAULT DELETED BY HANDLER
        AnimationManager.Singleton.ADD_ANIMATION(HandleBoardUpdateSyncOpponent(m)); // ADD THIS IENUMERATOR TO THE ANIMATION QUEUE
    }

    public static IEnumerator HandleBoardUpdateSyncOpponent(Message message)
    {
        OpponentBoard.Clear(); // CLEARS THE BOARD
        for (int i =0; i<2; i++){ // READS THE MESSAGE, AND FILLS THE BOARD TO BE EQUIVALENT TO WHAT WE HAVE PREVIOUSLY SEEN.
            for(int j=0; j<3; j++)
            {
                if (message.GetBool() == true)
                {
                    BoardCard c = Instantiate( OpponentBoard.NonDraggableCard, OpponentBoard.transform.position, OpponentBoard.transform.rotation);
                    c.Id = message.GetString();
                    c.Attack = message.GetUShort();
                    c.Hp = message.GetUShort();
                    c.x_location=i;
                    c.x_location=j;
                    OpponentBoard.SetAt(c,i,j);
                }
            }
        }
        yield break;
    }
```
There are times which we will not want to use the animation queue, but those will be niche cases, and will be adapted to account for. Take for instance the selection manager, which presents the client with cards to choose from. The client will choose a set of cards, and if the selection is valid, the server will terminate the window remotely, before moving on to the next game step.
```
    [MessageHandler((ushort) ServerToClient.selectionRequest)]  // RECEIVES SELECTION REQUEST
    public static void selectionRequest(Message message)
    {
        Message m = System.ObjectExtensions.Copy(message);
        AnimationManager.Singleton.ADD_ANIMATION(HandleSelectionRequest(m));
    }

    [MessageHandler((ushort) ServerToClient.confirmSelectionEnd)] // RECEIVES THE SELECTION CONFIRMATION
    public static void selectionEnd(Message message)
    {
        Debug.Log("SelectionEndMessage has been received");
        selectionDone = true; // TELLS THE CLIENT THAT THE SELECTION HAS BEEN MADE
        AnimationManager.Singleton.ADD_ANIMATION(NextSelectionRequestPrep()); // ADDS THIS TO THE QUEUE ORDER
    }
    public static IEnumerator NextSelectionRequestPrep() 
    {
        selectionDone = false; // ALLOWS A NEW SELECTION TO BE MADE
        yield break;
    }
    public static IEnumerator HandleSelectionRequest(Message message)
    {
        Singleton.SelectionPanel.SetActive(true);  // PRESENTS OPTIONS
        Singleton.Clear();
        
        ushort max = message.GetUShort(); // MAKES THE APPROPRIATE SELECTION WINDOW
        SelectionBuffer.Clear();
        minSelections = message.GetUShort();
        maxSelections = message.GetUShort();
        Debug.Log($"minSelections {minSelections} maxSelections {maxSelections}");
        for (ushort i=0; i < max; i++)
        {
            string id = message.GetString();
            Singleton.AddCard(id, i);
        }

   
        while (!selectionDone) // HOLDS THIS UNTIL THE SELECTION HAS BEEN MADE
        {
            yield return null;
        }
        Singleton.SelectionPanel.SetActive(false); // HIDES THE WINDOW AND EXITS
        yield break;
    }
```
## Server
The server is a great deal more complex and will be updated later.
