using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleDropdownHandler : MonoBehaviour
{

    public Dropdown dropdown;
    public string playerName; //has to be set before OnValueChanged is called

    public PlayerRoles role;
    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnValueChanged(int index)
    {

        if (shouldTrigger)
        {
            switch (dropdown.options[index].text.Trim())
            {
                case "Whitehat":
                    role = PlayerRoles.Whitehat;
                    break;
                case "Blackhat":
                    role = PlayerRoles.Blackhat;
                    break;
                case "Observer":
                    role = PlayerRoles.Observer;
                    break;
                default:
                    role = PlayerRoles.Whitehat;
                    break;
            }

            NewLobbyMgr.inst.OnValueChangedInRoleDropdown(playerName, role, dropdown, index);
        }


    }

    public bool shouldTrigger = true;
    public void SetValueWithoutTrigger(int val)
    {
        shouldTrigger = false;
        dropdown.value = val;
        dropdown.RefreshShownValue();
        shouldTrigger = true;
    }


}
