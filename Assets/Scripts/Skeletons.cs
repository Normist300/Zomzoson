using UnityEngine;

public class Skeletons : MonoBehaviour
{
    public void SingAndDance()
    {
        // Воспроизводим музыку скелетов
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Включаем анимацию скелетов
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    //    public void Subscribe(GridController gridController)
    //    {
    //        gridController.OnGridInitialized += HandleGridInitialized;
    //    }

    //    public void Unsubscribe(GridController gridController)
    //    {
    //        gridController.OnGridInitialized -= HandleGridInitialized;
    //    }
}
