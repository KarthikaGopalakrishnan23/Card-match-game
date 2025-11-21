using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public int cardId;       
    public Image frontImage; 
    public Image backImage;

    public bool isFlipped = false;
    public bool isMatched = false;

    float flipTime = 0.15f;

    public void Init(int id, Sprite front, Sprite back)
    {
        cardId = id;
        frontImage.sprite = front;
        backImage.sprite = back;

        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);

        isFlipped = false;
        isMatched = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMatched || isFlipped) return;

        StartCoroutine(FlipToFront());
        GameController.Instance.SelectCard(this);
    }

    IEnumerator FlipToFront()
    {
        isFlipped = true;
        GameController.Instance.PlayFlipSound();
       
        for (float t = 0; t < flipTime; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(1f, 0f, t / flipTime);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        


        backImage.gameObject.SetActive(false);
        frontImage.gameObject.SetActive(true);

        for (float t = 0; t < flipTime; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(0f, 1f, t / flipTime);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    public IEnumerator FlipBack()
    {
        isFlipped = false;

        for (float t = 0; t < flipTime; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(1f, 0f, t / flipTime);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);

        for (float t = 0; t < flipTime; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(0f, 1f, t / flipTime);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    public void SetMatched()
    {
        isMatched = true;
    }
}