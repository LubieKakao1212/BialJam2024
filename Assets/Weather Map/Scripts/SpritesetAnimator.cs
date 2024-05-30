using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SpritesetAnimator : MonoBehaviour
{
    public const int idleAnimationSpeed = 4;

    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private int rowsCount = 33;
    [SerializeField]
    private int columnsCount = 4;

    [SerializeField]
    private float animationSpeed;
    public float AnimationSpeed
    {
        get => animationSpeed;
        set => animationSpeed = value;
    }

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    [SerializeField, ReadOnly]
    private List<Sprite> currentSequence;

    private int currentAnimationIndex = -1;
    private bool isReversed;

    private int currentIndex;
    private float animationTimer;

    private void Awake()
    {
        currentSequence = new List<Sprite>();
    }

    private void Start()
    {
        SetAnimation(0);
        animationTimer = 0;
    }

    public void SetAnimation(int rowIndex, bool reversed = false)
    {
        SetAnimation(rowIndex, columnsCount, reversed);
    }

    public void SetAnimation(int rowIndex, int overrideSequenceLength, bool reversed = false)
    {
        if (currentAnimationIndex == rowIndex && overrideSequenceLength == currentSequence.Count && isReversed == reversed)
            return;

        isReversed = reversed;
        currentAnimationIndex = rowIndex;
        currentSequence.Clear();
        int firstFrameIndex = rowIndex * columnsCount;
        for (int i = 0; i < overrideSequenceLength; i++)
        {
            var relativeIndex = reversed ? (overrideSequenceLength - 1 - i) : i;
            currentSequence.Add(sprites[firstFrameIndex + relativeIndex]);
        }
        currentIndex = 0;
    }

    private void Update()
    {
        animationTimer += Time.deltaTime * animationSpeed;
        if (animationTimer > 1)
        {
            animationTimer -= 1;
            currentIndex = (currentIndex + 1) % currentSequence.Count;
            spriteRenderer.sprite = currentSequence[currentIndex];
        }
    }
}