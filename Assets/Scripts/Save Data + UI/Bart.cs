using UnityEngine;

public class Bart : MonoBehaviour
{
    public string[] badgesToAward;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (string badge in badgesToAward)
            {
                BatMan.instance.AwardBadge(badge);
            }
        }
    }
}
