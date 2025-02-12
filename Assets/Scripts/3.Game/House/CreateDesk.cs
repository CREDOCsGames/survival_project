using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDesk : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject createPanel;

    public void CanInteraction(bool _canInteraction)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        throw new System.NotImplementedException();
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        createPanel.GetComponent<CreatePanel>().SetCreateAcquisition(Acquisition.CraftTable);
        createPanel.SetActive(true);
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        throw new System.NotImplementedException();
    }

    public bool ReturnCanInteraction()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
