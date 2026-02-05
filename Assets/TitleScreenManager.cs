using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private PlayerController player;
    [SerializeField] private BossController boss;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        if (titlePanel != null)
        {
            titlePanel.SetActive(true);
        }
        
        if (player != null)
        {
            player.enabled = false;
        }
        
        if (boss != null)
        {
            boss.enabled = false;
        }
    }

    private void Update()
    {
        // Check if Fire button (Space) is pressed
        if (inputActions.Player.Fire.triggered && titlePanel != null && titlePanel.activeInHierarchy)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        titlePanel.SetActive(false);
        
        if (player != null)
        {
            player.enabled = true;
        }
        
        if (boss != null)
        {
            boss.enabled = true;
        }
    }
}