using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuildLesson
{
    public class SeparateManagerBuildLesson : MonoBehaviour
    {
        private static SeparateManagerBuildLesson instance;
        public static SeparateManagerBuildLesson Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<SeparateManagerBuildLesson>();
                return instance;
            }
        }
        private const float RADIUS = 8f;
        // public float DistanceFactor { get; set; }
        private const float DISTANCE_FACTOR = 0.00526488976f;

        // variable
        private int childCount;
        private Vector3 centerPosition;
        private Vector3 targetPosition;
        private float angle;
        public Button btnSeparate;
        private bool isSeparating;
        public bool IsSeparating 
        {
            get
            {
                return isSeparating;
            }
            set
            {
                isSeparating = value;
                btnSeparate.GetComponent<Image>().sprite = isSeparating ? Resources.Load<Sprite>(PathConfig.SEPARATE_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.SEPARATE_UNCLICK_IMAGE);
            }
        }

        public void HandleSeparate(bool isSeparating)
        {
            IsSeparating = isSeparating;
            if (IsSeparating)
            {
                btnSeparate.interactable = false;
                SeparateOrganModel();
                btnSeparate.interactable = true;
            }
            else
            {
                btnSeparate.interactable = false;
                BackToPositionOrgan();
                btnSeparate.interactable = true;
            }
        }

        public void SeparateOrganModel()
        {
            childCount = ObjectManagerBuildLesson.Instance.CurrentObject.transform.childCount;
            angle = (float)(360 / (childCount - 1));
            centerPosition = CalculateCentroid();

            for (int i = 0; i < childCount; i++)
            {
                targetPosition = ComputeTargetPosition(centerPosition, ObjectManagerBuildLesson.Instance.ListchildrenOfOriginPosition[i]);
                StartCoroutine(MoveObjectWithLocalPosition(ObjectManagerBuildLesson.Instance.CurrentObject.transform.GetChild(i).gameObject, targetPosition));
            }
        }

        private Vector3 CalculateCentroid()
        {
            Vector3 centroid = new Vector3(0, 0, 0);

            foreach (Vector3 localPosition in ObjectManagerBuildLesson.Instance.ListchildrenOfOriginPosition)
            {
                centroid += localPosition;
            }
            centroid /= ObjectManagerBuildLesson.Instance.ListchildrenOfOriginPosition.Count;
            return centroid;
        }

        public Vector3 ComputeTargetPosition(Vector3 center, Vector3 currentPosition)
        {
            Vector3 dir = currentPosition - center;
            return dir.normalized * DISTANCE_FACTOR;
            // return dir.normalized * DistanceFactor;
        }

        public IEnumerator MoveObjectWithLocalPosition(GameObject moveObject, Vector3 targetPosition)
        {
            float timeSinceStarted = 0f;
            while (true)
            {
                timeSinceStarted += Time.deltaTime;
                moveObject.transform.localPosition = Vector3.Lerp(moveObject.transform.localPosition, targetPosition, timeSinceStarted);
                if (moveObject.transform.localPosition == targetPosition)
                {
                    yield break;
                }
                yield return null;
            }
        }

        public void BackToPositionOrgan()
        {
            // if (ObjectManagerBuildLesson.Instance.ListchildrenOfOriginPosition.Count < 1)	        
            // {
            //     return;
            // }
            int childCount = ObjectManagerBuildLesson.Instance.CurrentObject.transform.childCount;
            if (childCount < 0)
            {
                return;
            }
            for (int i = 0; i < childCount; i++)
            {
                targetPosition = ObjectManagerBuildLesson.Instance.ListchildrenOfOriginPosition[i];	
                StartCoroutine(MoveObjectWithLocalPosition(ObjectManagerBuildLesson.Instance.CurrentObject.transform.GetChild(i).gameObject, targetPosition));
            }
        }
    }
}
