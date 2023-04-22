using UnityEngine;
using UnityEngine.UI;

public class DropdownMenu : MonoBehaviour
{
    Dropdown m_Dropdown;

    void Start()
    {
        m_Dropdown = GetComponent<Dropdown>();
        //Add listener for when the value of the Dropdown changes, to take action
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
        

        //Initialise the Text to say the first value of the Dropdown
    }

    //Ouput the new value of the Dropdown into Text
    void DropdownValueChanged(Dropdown change)
    {

    }
}
