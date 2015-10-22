﻿using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.MathLib;
using System;
using System.Globalization;


namespace bytePassion.Lib.GeometryLib.Base
{
    public struct Angle
	{
		// TODO get culture from framework
		private static readonly IFormatProvider Numberformat = CultureInfo.CreateSpecificCulture("en-US");

		public static readonly Angle Zero = new Angle(0);

		private double valueAsRad;		//
		private double sin;				//	variables to avoid
		private double cos;				//  redundant computation
		private double tan;				//

		public Angle (double angle, AngleUnit representation = AngleUnit.Degree)
		{
			if (representation == AngleUnit.Radians)
				angle = (180.0/Math.PI)*angle;

			Value = angle%360.0;

			valueAsRad = Double.NaN;
			sin = Double.NaN;
			cos = Double.NaN;
			tan = Double.NaN;
		}

        public double Value { get; }

		public double ValueAsRad => valueAsRad = Double.IsNaN(valueAsRad) ? (Math.PI/180.0)*Value : valueAsRad;

		public Angle PosValue => Value >= 0 ? this : new Angle(360 + Value);
		public Angle Inverted => new Angle(-Value);

		public double Sin => sin = Double.IsNaN(sin) ? Math.Sin(ValueAsRad) : sin;
		public double Cos => cos = Double.IsNaN(cos) ? Math.Cos(ValueAsRad) : cos;
		public double Tan => tan = Double.IsNaN(cos) ? Math.Tan(ValueAsRad) : tan;

        public override bool   Equals (object obj) => this.Equals(obj, (a1, a2) => GeometryLibUtils.DoubleEquals(a1.PosValue.Value, a2.PosValue.Value));
		public override int    GetHashCode ()      => Value.GetHashCode();
	    public override string ToString()          => GeometryLibUtils.DoubleFormat(Value);

        public static bool operator >  (Angle a1, Angle a2) => a1.Value > a2.Value;
		public static bool operator >= (Angle a1, Angle a2) => a1 > a2 || a1 == a2;
		public static bool operator <  (Angle a1, Angle a2) => !(a1 >= a2);
		public static bool operator <= (Angle a1, Angle a2) => !(a1 > a2);
		public static bool operator == (Angle a1, Angle a2) => a1.Equals(a2);
		public static bool operator != (Angle a1, Angle a2) => !(a1 == a2);

		public static Angle  operator + (Angle a1, Angle a2) => new Angle(a1.Value + a2.Value);
		public static Angle  operator - (Angle a1, Angle a2) => a1 + (-a2);
		public static Angle  operator - (Angle a)            => new Angle(-a.Value);
		public static Angle  operator * (Angle a, double d)  => new Angle(a.Value*d);
		public static Angle  operator * (double d, Angle a)  => a*d;
		public static Angle  operator / (Angle a, double d)  => a*(1.0/d);
		public static double operator / (Angle a1, Angle a2) => a1.Value/a2.Value;

        public static Angle MinimalAngleBetween (Angle a1, Angle a2)
		{
			var interval = new AngleInterval(a1, a2);

			return (interval.AbsolutAngleValue.Value > 180) 
				? new Angle(360 - interval.AbsolutAngleValue.Value) 
				: interval.AbsolutAngleValue;
		}

        public static Angle Parse (string s)
		{
			var parts = s.Split(' ');

			if (parts.Length != 2)
				throw new ArgumentException(s + " cannot be converted to an Angle");

			var number = Double.Parse(parts[0].Trim(), Numberformat);
			var unit = parts[1].Trim() == "deg" ? AngleUnit.Degree : AngleUnit.Radians;

			return new Angle(number, unit);
		}
	}
}