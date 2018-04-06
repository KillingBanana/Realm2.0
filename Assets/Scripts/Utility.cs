﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class Utility {
	public static string Capitalize(this string s) => string.Concat(s.First().ToString().ToUpper(), s.Substring(1));
	public static string ToTitleCase(this string s) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);

	public static T RandomItem<T>(this IList<T> list, Random random = null) => list[(random ?? GameController.Random).Next(list.Count)];

	public static T RandomValue<T>() where T : IConvertible, IFormattable, IComparable => ((T[]) Enum.GetValues(typeof(T))).RandomItem();

	public static int Abs(this int i) => i < 0 ? -i : i;
	public static float Abs(this float i) => i < 0 ? -i : i;

	public static int Sign(this int i) => i > 0 ? 1 : i < 0 ? -1 : 0;
	public static float Sign(this float f) => f > 0 ? 1 : f < 0 ? -1 : 0;

	public static bool Contains(this Vector2 v2, float f) => v2.x <= f && f <= v2.y;
	public static float Average(this Vector2 v2) => (v2.x + v2.y) / 2;

	public static bool RandomBool => GameController.Random.NextDouble() > 0.5f;
}