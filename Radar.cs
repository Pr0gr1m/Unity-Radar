using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour
{
    [Header("World Radar")]
    [SerializeField] private int radarFOV = 30;
    [SerializeField] private int radarAngle = 0;

    [SerializeField] private int radarAngleMin;
    [SerializeField] private int radarIncrement = 1;

    [SerializeField] private float MaxDist;

    [SerializeField] private bool openRadar;

    [Header("Incrementation")]
    [SerializeField] private float wait;
    [SerializeField] private float error;

    [Header("Visual")]
    [SerializeField] private GameObject triangleVisual;
    [SerializeField] private GameObject triangleNotVisual;

    [Header("UI")]
    [SerializeField] private GameObject radarPointGameObject;
    [SerializeField] private GameObject playerPointObject;
    [SerializeField] private GameObject canvasRadarPanel;

    [SerializeField] private Text maxDistText;
    [SerializeField] private Text halfDistText;

    [SerializeField] private GameObject FOVLineA;
    [SerializeField] private GameObject FOVLineB;

    [SerializeField] private GameObject RadarLine;

    private List<Vector2> registeredPositions;

    private void Awake()
    {
        registeredPositions = new List<Vector2>();
        StartCoroutine(RadarIncrementation());

        FOVLineA.transform.Rotate(0, 0, radarAngleMin, Space.Self);
        FOVLineB.transform.Rotate(0, 0, -radarAngleMin, Space.Self);
    }

    private void LateUpdate()
    {
        if (!openRadar)
        {
            FOVLineA.transform.eulerAngles = Vector3.zero;
            FOVLineB.transform.eulerAngles = Vector3.zero;

            FOVLineA.transform.Rotate(0, 0, radarAngleMin, Space.Self);
            FOVLineB.transform.Rotate(0, 0, -radarAngleMin, Space.Self);
        } else
        {
            FOVLineA.gameObject.SetActive(false);
            FOVLineB.gameObject.SetActive(false);
        }

        RadarLine.transform.eulerAngles = Vector3.zero;
        RadarLine.transform.Rotate(0, 0, radarAngle, Space.Self);

        maxDistText.text = MaxDist.ToString();
        halfDistText.text = (MaxDist/2).ToString();
    }

    private IEnumerator RadarIncrementation()
    {
        while (true)
        {
            triangleVisual.transform.rotation = Quaternion.Euler(triangleVisual.transform.rotation.x, triangleVisual.transform.rotation.y, radarAngle);
            CheckRadar();

            if (!openRadar)
            {
                if (Mathf.Abs(radarAngle) >= radarAngleMin)
                {
                    radarAngle = radarIncrement == 1 ? radarAngleMin : -radarAngleMin;
                    radarIncrement *= -1;
                    registeredPositions.Clear();
                }
            }

            yield return new WaitForSeconds(wait);

            radarAngle += radarIncrement;

            if (radarAngle > 360)
            {
                registeredPositions.Clear();
                radarAngle -= 360;
            }
        }
    }

    private void CheckRadar()
    {
        for (int i = -radarFOV / 2; i <= radarFOV / 2; i++)
        {
            float angle = i + radarAngle;

            triangleNotVisual.transform.eulerAngles = new Vector3(triangleVisual.transform.eulerAngles.x, triangleVisual.transform.eulerAngles.y, angle);

            RaycastHit2D hit;
            hit = Physics2D.Raycast(triangleVisual.transform.position, triangleNotVisual.transform.up, MaxDist);

            if (hit)
            {
                if (IsPointInTheList(hit.point, error))
                {
                    return;
                }

                registeredPositions.Add(hit.point);

                Vector2 radarPos = transform.position;
                Vector2 offsetHitPointFromRadarWorldPos = hit.point - radarPos;

                float screenMaxDistYHeight = maxDistText.GetComponent<RectTransform>().anchoredPosition.y - playerPointObject.GetComponent<RectTransform>().anchoredPosition.y;
                float screenPointHeight = screenMaxDistYHeight * (Vector2.Distance(radarPos, hit.point) / MaxDist);

                GameObject radarpoint = Instantiate(radarPointGameObject, canvasRadarPanel.transform);

                radarpoint.GetComponent<RectTransform>().anchoredPosition = playerPointObject.GetComponent<RectTransform>().anchoredPosition;

                radarpoint.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, screenPointHeight);

                radarpoint.GetComponent<RectTransform>().Rotate(0, 0, radarAngle);
                radarpoint.transform.GetChild(0).GetComponent<RectTransform>().Rotate(0, 0, -radarAngle);
            }
        }
    }

    private bool IsPointInTheList(Vector2 targetPoint, float error = 5)
    {
        foreach(Vector2 element in registeredPositions)
        {
            if(Vector2.Distance(element, targetPoint) <= error)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(triangleNotVisual.transform.position, triangleNotVisual.transform.position + 50*triangleNotVisual.transform.up);
    }
}
