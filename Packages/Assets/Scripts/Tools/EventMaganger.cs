using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventsArrayEventsManager
{
    public string nameList;
    public UnityEvent events;
    
}

public class EventMaganger : MonoBehaviour
{

    [SerializeField] public EventsArrayEventsManager[] EventsArray;


    public void StartEvents(string nameList)
    {
        foreach (EventsArrayEventsManager eventList in EventsArray)
        {
            if (eventList.nameList == nameList)
            {
                eventList.events.Invoke();
            }
        }
    }

}

