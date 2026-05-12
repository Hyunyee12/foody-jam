using UnityEngine;
using System.Collections;

public class BlockMovement : MonoBehaviour
{
    [Header("Grid & Board Settings")]
    public Vector2 gridSize = new Vector2(1f, 1f);
    public Vector2 gridOffset = new Vector2(0f, 0f);
    public Vector2 snapOffset = new Vector2(0f, 0f); // 블록 크기에 맞춘 미세 조정 값
    public float returnSpeed = 15f;
    public BoxCollider2D boardCollider;

    private Vector3 originalPosition;
    private Vector3 dragStartMousePos;
    private bool isDragging = false;

    private BoxCollider2D myCollider;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private BlockData myData;

    private enum DragAxis { None, Horizontal, Vertical }
    private DragAxis currentAxis = DragAxis.None;

    void Awake()
    {
        ForceLoadComponents();

        if (spriteRenderer != null) originalSortingOrder = spriteRenderer.sortingOrder;
        if (boardCollider == null) boardCollider = GameObject.Find("board")?.GetComponent<BoxCollider2D>();
    }

    private void ForceLoadComponents()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (myData == null) myData = GetComponent<BlockData>();
    }

    void OnMouseDown()
    {
        ForceLoadComponents();

        if (isDragging || myCollider == null) return;

        originalPosition = transform.position;
        dragStartMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragStartMousePos.z = 0f;
        currentAxis = DragAxis.None;
        isDragging = true;

        if (spriteRenderer != null) spriteRenderer.sortingOrder = 100;
    }

    void OnMouseDrag()
    {
        ForceLoadComponents();

        if (!isDragging || myCollider == null) return;

        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0f;
        Vector3 mouseDelta = currentMousePos - dragStartMousePos;

        if (currentAxis == DragAxis.None && mouseDelta.magnitude > 0.1f)
        {
            if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y)) currentAxis = DragAxis.Horizontal;
            else currentAxis = DragAxis.Vertical;
        }

        Vector3 targetPosition = originalPosition;
        if (currentAxis == DragAxis.Horizontal) targetPosition.x = originalPosition.x + mouseDelta.x;
        else if (currentAxis == DragAxis.Vertical) targetPosition.y = originalPosition.y + mouseDelta.y;
        else return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPosition, myCollider.bounds.size * 0.8f, 0f);
        bool hitObstacle = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != this.gameObject && (boardCollider == null || hit.gameObject != boardCollider.gameObject))
            {
                BlockData hitData = hit.GetComponent<BlockData>();
                if (CanMerge(hitData))
                {
                    // ★ 수정됨: 0.5f 기준이 너무 깐깐해서 안 합쳐지던 문제를 1.2f로 늘려 해결!
                    float distance = Vector2.Distance(targetPosition, hit.transform.position);
                    if (distance < 1.2f) 
                    {
                        ExecuteMerge(hit.gameObject);
                        return;
                    }
                }
                else
                {
                    hitObstacle = true;
                    break;
                }
            }
        }

        if (hitObstacle)
        {
            isDragging = false;
            if (spriteRenderer != null) spriteRenderer.sortingOrder = originalSortingOrder;
            StartCoroutine(ReturnToOriginalPosition());
            return;
        }

        if (boardCollider != null)
        {
            Bounds b = boardCollider.bounds;
            float hX = myCollider.bounds.extents.x;
            float hY = myCollider.bounds.extents.y;
            targetPosition.x = Mathf.Clamp(targetPosition.x, b.min.x + hX, b.max.x - hX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, b.min.y + hY, b.max.y - hY);
        }

        transform.position = targetPosition;
    }

    void OnMouseUp()
    {
        ForceLoadComponents();

        if (!isDragging || myCollider == null) return;
        isDragging = false;
        currentAxis = DragAxis.None;
        if (spriteRenderer != null) spriteRenderer.sortingOrder = originalSortingOrder;

        Vector3 snapPosition = GetSnapPosition(transform.position, true);

        Collider2D[] upHits = Physics2D.OverlapBoxAll(snapPosition, myCollider.bounds.size * 0.5f, 0f);
        bool upHitObstacle = false;

        foreach (Collider2D hit in upHits)
        {
            if (hit.gameObject != this.gameObject && (boardCollider == null || hit.gameObject != boardCollider.gameObject))
            {
                BlockData hitData = hit.GetComponent<BlockData>();
                if (CanMerge(hitData))
                {
                    // ★ 수정됨: 마우스를 놨을 때도 1.2f 범위 안에 들어오면 합체!
                    float distance = Vector2.Distance(snapPosition, hit.transform.position);
                    if (distance < 1.2f)
                    {
                        ExecuteMerge(hit.gameObject);
                        return;
                    }
                }
                upHitObstacle = true;
                break;
            }
        }

        if (upHitObstacle) StartCoroutine(ReturnToOriginalPosition());
        else transform.position = snapPosition;
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        while (timer < 1f)
        {
            timer += Time.deltaTime * returnSpeed;
            transform.position = Vector3.Lerp(startPos, originalPosition, timer);
            yield return null;
        }
        transform.position = originalPosition;
    }

    private bool CanMerge(BlockData targetData)
    {
        if (myData == null || targetData == null) return false;

        BlockData tool = myData.myType == BlockData.BlockType.Tool ? myData : (targetData.myType == BlockData.BlockType.Tool ? targetData : null);
        BlockData material = myData.myType == BlockData.BlockType.Material ? myData : (targetData.myType == BlockData.BlockType.Material ? targetData : null);

        if (tool == null || material == null) return false;
        if (material.compatibleTools.Contains(tool.myToolKind)) return true;

        return false;
    }

    private void ExecuteMerge(GameObject hitBlock)
    {
        BlockData targetData = hitBlock.GetComponent<BlockData>();
        BlockData material = myData.myType == BlockData.BlockType.Material ? myData : targetData;

        Vector3 mySize = transform.localScale;
        Vector3 targetSize = hitBlock.transform.localScale;
        Vector3 biggerSize = (mySize.x * mySize.y > targetSize.x * targetSize.y) ? mySize : targetSize;

        Vector3 spawnPos = hitBlock.transform.position;
        spawnPos.z = 0f;

        if (material.mergedPrefab != null)
        {
            GameObject newBlock = Instantiate(material.mergedPrefab, spawnPos, Quaternion.identity);
            newBlock.transform.localScale = biggerSize;
        }

        Destroy(hitBlock);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int x = -10; x <= 10; x++)
        {
            for (int y = -10; y <= 10; y++)
            {
                float centerX = x * gridSize.x + gridOffset.x;
                float centerY = y * gridSize.y + gridOffset.y;
                Vector3 center = new Vector3(centerX, centerY, 0);
                Gizmos.DrawWireCube(center, new Vector3(gridSize.x, gridSize.y, 0));
            }
        }
    }

    private Vector3 GetSnapPosition(Vector3 currentPos, bool clampToBoard)
    {
        float xIdx = Mathf.Floor((currentPos.x - gridOffset.x - snapOffset.x) / gridSize.x + 0.5f);
        float yIdx = Mathf.Floor((currentPos.y - gridOffset.y - snapOffset.y) / gridSize.y + 0.5f);

        Vector2 snapPos = new Vector2(
            xIdx * gridSize.x + gridOffset.x + snapOffset.x,
            yIdx * gridSize.y + gridOffset.y + snapOffset.y
        );

        if (clampToBoard && boardCollider != null && myCollider != null)
        {
            Bounds b = boardCollider.bounds;
            float hX = myCollider.bounds.extents.x;
            float hY = myCollider.bounds.extents.y;
            
            snapPos.x = Mathf.Clamp(snapPos.x, b.min.x + hX, b.max.x - hX);
            snapPos.y = Mathf.Clamp(snapPos.y, b.min.y + hY, b.max.y - hY);
            
            xIdx = Mathf.Floor((snapPos.x - gridOffset.x - snapOffset.x) / gridSize.x + 0.5f);
            yIdx = Mathf.Floor((snapPos.y - gridOffset.y - snapOffset.y) / gridSize.y + 0.5f);
            snapPos.x = xIdx * gridSize.x + gridOffset.x + snapOffset.x;
            snapPos.y = yIdx * gridSize.y + gridOffset.y + snapOffset.y;
        }

        return new Vector3(snapPos.x, snapPos.y, currentPos.z);
    }

    [ContextMenu("★ 그리드에 정렬하기 (Snap to Grid)")]
    private void SnapInEditor()
    {
        ForceLoadComponents();
        transform.position = GetSnapPosition(transform.position, false);
    }
}