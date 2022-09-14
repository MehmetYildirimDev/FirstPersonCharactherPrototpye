using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIScript : MonoBehaviour
{

    [SerializeField] private Text HealtText = default;

    private void OnEnable()
    {
        FirstPersonController.onDamage += UpdateHealt;
        FirstPersonController.onHeal += UpdateHealt;
    }
    private void OnDisable()
    {
        FirstPersonController.onDamage -= UpdateHealt;
        FirstPersonController.onHeal -= UpdateHealt;
    }

    private void Start()
    {
        UpdateHealt(100);
    }

    private void UpdateHealt(float currentHealt)
    {
        HealtText.text = currentHealt.ToString();
    }
}
