using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geograpny
{
    public class BoundsManager : MonoBehaviour
    {
        [SerializeField] EdgeCollider2D _mapBoundsCollider;

        private void Awake()
        {
            var poly = gameObject.AddComponent<PolygonCollider2D>();
            List<Vector2> points = new List<Vector2>();
            foreach (var point in poly.points)
                points.Add(point);
            points.Add(new Vector2(points[0].x, points[0].y));
            _mapBoundsCollider = gameObject.AddComponent<EdgeCollider2D>();
            _mapBoundsCollider.points = points.ToArray();
            Destroy(poly);
        }

        public Bounds GetMapBounds()
        {
            return _mapBoundsCollider.bounds;
        }
    }

}
