using System;
using System.Collections.Generic;
using System.Linq;

public static class Utility {

	private static Random DefaultRandom => GameController.Random;

	public static string Capitalize(this string s) => s.First().ToString().ToUpper() + s.Substring(1);

	public static T RandomItem<T>(this IList<T> list, int startRank = 0) => RandomItem(list, DefaultRandom, startRank);
	public static T RandomItem<T>(this IList<T> list, Random random, int startRank = 0) => list[random.Next(startRank, list.Count)];
	public static T RandomValue<T>(int startRank = 0) where T : IConvertible, IFormattable, IComparable => ((T[]) Enum.GetValues(typeof(T))).RandomItem(startRank);

	public static int Abs(this int i) => i < 0 ? -i : i;
	public static float Abs(this float i) => i < 0 ? -i : i;

	public static int Sign(this int i) => i > 0 ? 1 : i < 0 ? -1 : 0;

	public static float Sign(this float f) => f > 0 ? 1 : f < 0 ? -1 : 0;

	public static bool RandomBool => UnityEngine.Random.value > 0.5f;
}