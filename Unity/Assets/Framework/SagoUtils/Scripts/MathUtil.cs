namespace SagoUtils {

	using UnityEngine;

	public class MathUtil {

		/// <summary>
		/// The square root of 2 pi.
		/// </summary>
		public const float SQRT_TWO_PI = 2.506628274631001f;

		static public Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {

			float tt;
			tt = t * t;

			float u;
			u = 1 - t;
			
			float uu;
			uu = u * u;

			Vector3 p;
			p = uu * p0;
			p += 2 * u * t * p1;
			p += tt * p2;
			
			return p;

		}

		static public Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {

			float tt;
			tt = t * t;

			float u;
			u = 1 - t;

			float uu;
			uu = u * u;
			
			return
				u * uu * p0 +
				3 * uu * t * p1 +
				3 * u * tt * p2 +
				tt * t * p3;
			
		}
		
		static public Vector3 BezierTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			
			float tt;
			tt = t * t;
			
			float u;
			u = 1 - t;
			
			float uu;
			uu = u * u;
			
			float ut;
			ut = u * t;
			
			return
				-3 * uu * p0 +
				(3 * uu - 6 * ut) * p1 +
				(6 * ut - 3 * tt) * p2 +
				(3 * tt) * p3;
			
		}
		
		static public float RandomSign() {
			return Mathf.Pow(-1, Random.Range(0, 2));
		}

		static public float SignedAngle(Vector3 from, Vector3 to) {
			return SignedAngle(from, to, Vector3.forward);
		}

		static public float SignedAngle(Vector3 from, Vector3 to, Vector3 positiveAxis) {

			Vector3 cross;
			cross = Vector3.Cross(from, to);

			// Bug in Unity Vector3.Angle returns non-zero in some cases 
			// even when from and to are equal.
			if (cross.sqrMagnitude == 0f) {
				return 0f;
			}

			float angle;
			angle = Mathf.Abs(Vector3.Angle(from, to));

			int sign;
			sign = (Vector3.Dot(cross, positiveAxis) < 0) ? -1 : 1;

			return sign * angle;

		}

		static public Vector2 PolarToCartesian(Polar polar) {
			return Quaternion.AngleAxis(Mathf.Rad2Deg * polar.Theta, Vector3.forward) * (polar.Radius * Vector3.right);
		}

		static public Polar CartesianToPolar(Vector2 cartesian) {

			Polar polar;
			polar = new Polar();
			polar.Radius = cartesian.magnitude;
			polar.Theta = Mathf.Atan2(cartesian.y, cartesian.x);

			return polar;

		}

		static public Vector3 SphericalToCartesian(Spherical spherical) {
			return SphericalToCartesian(spherical.Radius, spherical.Theta, spherical.Phi);
		}

		static public Vector3 SphericalToCartesian(float r, float theta, float phi) {
			
			float cosTheta;
			cosTheta = Mathf.Cos(theta);
			
			float sinTheta;
			sinTheta = Mathf.Sin(theta);
			
			float cosPhi;
			cosPhi = Mathf.Cos(phi);
			
			float sinPhi;
			sinPhi = Mathf.Sin(phi);
			
			Vector3 point;
			point = Vector3.zero;
			point.x = r * cosTheta * sinPhi;
			point.y = r * sinTheta * sinPhi;
			point.z = r * cosPhi;
			
			return point;
			
		}

		static public Spherical CartesianToSpherical(Vector3 cartesian) {

			Spherical spherical;
			spherical = new Spherical();
			spherical.Radius = cartesian.magnitude;
			spherical.Theta = Mathf.Atan2(cartesian.y, cartesian.x);
			spherical.Phi = Mathf.Acos(cartesian.z / spherical.Radius);

			return spherical;

		}

		static public float PositiveAngle(float degrees) {
			return WrappedPeriodic(degrees, 360, 180);
		}

		static public float WrappedAngle(float degrees) {
			return WrappedPeriodic(degrees, 360);
		}

		static public float WrappedPeriodic(float t, float period) {
			return WrappedPeriodic(t, period, 0);
		}

		static public float WrappedPeriodic(float t, float period, float shift) {

			period = Mathf.Abs(period);

			float halfPeriod;
			halfPeriod = 0.5f * period;

			t -= shift;
			t %= period;

			if (t <= -halfPeriod) t += period;
			else if (t > halfPeriod) t -= period;

			return t + shift;

		}

		/// <summary>
		/// Value of a Gaussian distribution (bell curve, normal distribution) with mean of 0 and standard
		/// deviation of 1, at the given independent coordinate, x.
		/// </summary>
		/// <param name="x">The independent coordinate.</param>
		static public float Gaussian01(float x) {
			return Mathf.Exp(-0.5f * x * x) / SQRT_TWO_PI;
		}

		/// <summary>
		/// Value of a Gaussian distribution (bell curve, normal distribution) with the given mean and standard
		/// deviation, at the given independent coordinate, x.
		/// </summary>
		/// <param name="x">The independent coordinate.</param>
		static public float Gaussian(float mean, float standardDeviation, float x) {
			return Gaussian01((x - mean) / standardDeviation) / standardDeviation;
		}
	}


	public struct Polar {
		public float Radius;
		public float Theta;
	}


	public struct Spherical {
		public float Radius;
		public float Theta;
		public float Phi;
	}


}
