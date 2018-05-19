namespace Juice.Utils {

	using SagoUtils;
	using UnityEngine;
	using System.Collections.Generic;

	public class BezierPath {


		#region Constructor

		public BezierPath() {
			ControlPoints = new List<Vector2>();
		}

		#endregion


		#region Methods

		public void Interpolate(List<Vector2> segmentPoints, float curvatureScale) {

			ControlPoints.Clear();

			if (segmentPoints.Count == 0) {
				segmentPoints.Add(Vector2.zero);
			}

			if (segmentPoints.Count == 1) {
				segmentPoints.Add(segmentPoints[0]);
			}

			Vector3 p0;
			Vector3 p1;
			Vector3 p2;
			Vector3 q0;
			Vector3 q1;
			Vector3 tangent;

			for (int i = 0; i < segmentPoints.Count; i++) {

				if (i == 0) {

					p1 = segmentPoints[i];
					p2 = segmentPoints[i + 1];
					tangent = (p2 - p1);
					q1 = p1 + curvatureScale * tangent;

					ControlPoints.Add(p1);
					ControlPoints.Add(q1);

				} else if (i == segmentPoints.Count - 1) {

					p0 = segmentPoints[i - 1];
					p1 = segmentPoints[i];
					tangent = (p1 - p0);
					q0 = p1 - curvatureScale * tangent;

					ControlPoints.Add(q0);
					ControlPoints.Add(p1);

				} else {

					p0 = segmentPoints[i - 1];
					p1 = segmentPoints[i];
					p2 = segmentPoints[i + 1];
					tangent = (p2 - p0).normalized;
					q0 = p1 - curvatureScale * tangent * (p1 - p0).magnitude;
					q1 = p1 + curvatureScale * tangent * (p2 - p1).magnitude;

					ControlPoints.Add(q0);
					ControlPoints.Add(p1);
					ControlPoints.Add(q1);

				}

			}

			CurveCount = (ControlPoints.Count - 1) / 3;

		}

		public Vector2 CalculateBezierPoint(float tPath) {

			int curveIndex;
			curveIndex = Mathf.Min(Mathf.FloorToInt(tPath * CurveCount), CurveCount - 1);

			float tPathMin;
			tPathMin = curveIndex / (CurveCount * 1.0f);

			float tPathMax;
			tPathMax = (curveIndex + 1) / (CurveCount * 1.0f);

			float tCurve;
			tCurve = (tPath - tPathMin) / (tPathMax - tPathMin);

			return CalculateBezierPoint(curveIndex, tCurve);

		}

		#endregion


		#region Private Properties

		private List<Vector2> ControlPoints {
			get;
			set;
		}

		private int CurveCount {
			get;
			set;
		}

		#endregion


		#region Functions

		private Vector2 CalculateBezierPoint(int curveIndex, float t) {

			int nodeIndex;
			nodeIndex = curveIndex * 3;

			Vector2 p0;
			p0 = ControlPoints[nodeIndex];

			Vector2 p1;
			p1 = ControlPoints[nodeIndex + 1];

			Vector2 p2;
			p2 = ControlPoints[nodeIndex + 2];

			Vector2 p3;
			p3 = ControlPoints[nodeIndex + 3];

			return MathUtil.Bezier(p0, p1, p2, p3, t);

		}

		#endregion


	}

}