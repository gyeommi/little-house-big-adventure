using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : BaseCharacterHealth
{
    protected override void Die()
    {
        base.Die();

        Debug.Log("플레이어 사망! GameOverScene으로 이동합니다.");

        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }
}