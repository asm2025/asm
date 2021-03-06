﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Bogus;
using Bogus.DataSets;
using JetBrains.Annotations;
using System.ServiceProcess;
using essentialMix;
using essentialMix.Newtonsoft.Helpers;
using essentialMix.Patterns.Threading;
using essentialMix.Threading;
using essentialMix.Collections;
using essentialMix.Comparers;
using essentialMix.Cryptography;
using essentialMix.Cryptography.Settings;
using essentialMix.Exceptions;
using essentialMix.Extensions;
using essentialMix.Helpers;
using Other.Microsoft.Collections;
using essentialMix.Threading.Collections.ProducerConsumer;
using essentialMix.Threading.Helpers;
using Newtonsoft.Json;
using TimeoutException = System.TimeoutException;
using Menu = EasyConsole.Menu;
using static Crayon.Output;

// ReSharper disable UnusedMember.Local
namespace TestApp
{
	internal class Program
	{
		private const int START = 10;
		private const int SMALL = 10_000;
		private const int MEDIUM = 100_000;
		private const int HEAVY = 1_000_000;

		private static readonly string __compilationText = Yellow($@"
This is C# (a compiled language), so the test needs to run at least
once before considering results in order for the code to be compiled
and run at full speed. The first time this test run, it will start 
with just {START} items and the next time when you press '{Bright.Green("Y")}', it will 
work with {HEAVY} items.");

		private static readonly Lazy<Faker> __fakeGenerator = new Lazy<Faker>(() => new Faker(), LazyThreadSafetyMode.PublicationOnly);
		private static readonly string[] __sortAlgorithms = 
		{
			nameof(IListExtension.SortBubble),
			nameof(IListExtension.SortSelection),
			nameof(IListExtension.SortInsertion),
			nameof(IListExtension.SortHeap),
			nameof(IListExtension.SortMerge),
			nameof(IListExtension.SortQuick),
			nameof(IListExtension.SortShell),
			nameof(IListExtension.SortComb),
			nameof(IListExtension.SortTim),
			nameof(IListExtension.SortCocktail),
			nameof(IListExtension.SortBitonic),
			nameof(IListExtension.SortPancake),
			nameof(IListExtension.SortBinary),
			nameof(IListExtension.SortGnome),
			nameof(IListExtension.SortBrick),
		};

		private static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;

			//TestDomainName();

			//TestFibonacci();
			//TestGroupAnagrams();
			//TestKadaneMaximum();
			//TestLevenshteinDistance();
			//TestDeepestPit();

			//TestThreadQueue();

			//TestSortAlgorithm();
			//TestSortAlgorithms();

			//TestLinkedQueue();
			//TestMinMaxQueue();

			//TestSinglyLinkedList();
			//TestLinkedList();

			//TestDeque();
			//TestLinkedDeque();

			//TestBinaryTreeFromTraversal();
			
			//TestBinarySearchTreeAdd();
			//TestBinarySearchTreeRemove();
			//TestBinarySearchTreeBalance();
			//TestBinaryTreeFindClosest();
			//TestBinaryTreeBranchSums();
			//TestBinaryTreeInvert();

			//TestAVLTreeAdd();
			//TestAVLTreeRemove();
			
			//TestRedBlackTreeAdd();
			//TestRedBlackTreeRemove();

			//TestAllBinaryTrees();
			//TestAllBinaryTreesFunctionality();
			//TestAllBinaryTreesPerformance();

			//TestSortedSetPerformance();

			//TestTreeEquality();

			//TestTrie();
			//TestTrieSimilarWordsRemoval();

			//TestSkipList();

			//TestDisjointSet();
			
			//TestBinaryHeapAdd();
			//TestBinaryHeapRemove();
			//TestBinaryHeapElementAt();
			//TestBinaryHeapDecreaseKey();
		
			//TestBinomialHeapAdd();
			//TestBinomialHeapRemove();
			//TestBinomialHeapElementAt();
			//TestBinomialHeapDecreaseKey();
			
			//TestPairingHeapAdd();
			//TestPairingHeapRemove();
			//TestPairingHeapElementAt();
			//TestPairingHeapDecreaseKey();
			
			//TestFibonacciHeapAdd();
			//TestFibonacciHeapRemove();
			//TestFibonacciHeapElementAt();
			//TestFibonacciHeapDecreaseKey();
			
			// todo IndexMin not working yet!!!
			//TestIndexMinAdd();
			//TestIndexMinRemove();
			//TestIndexMinElementAt();
			//TestIndexMinDecreaseKey();

			//TestAllHeapsPerformance();

			//TestGraph();

			//TestAsymmetric();

			//TestSingletonAppGuard();

			//TestImpersonationHelper();
			
			//TestServiceHelper();
			
			//TestUriHelper();
			//TestUriHelperRelativeUrl();
			
			//TestJsonUriConverter();

			TestDevicesMonitor();

			ConsoleHelper.Pause();
		}

		private static void TestDomainName()
		{
			string[] domains =
			{
				"https://stackoverflow.com/questions/4643227/top-level-domain-from-url-in-c-sharp",
				"https://stackoverflow.com/questions/3121957/how-can-i-do-a-case-insensitive-string-comparison",
				"https://github.com/nager/Nager.PublicSuffix",
				"https://docs.microsoft.com/en-us/dotnet/csharp/how-to/compare-strings",
				"https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators"
			};
			List<(string, string)> matchingDomains = new List<(string, string)>();
			Title("Testing domain names...");

			for (int i = 0; i < domains.Length - 1; i++)
			{
				string x = domains[i];

				for (int j = i + 1; j < domains.Length; j++)
				{
					string y = domains[j];
					Console.WriteLine(Bright.Black("Testing:"));
					Console.WriteLine(x);
					Console.WriteLine(y);
					bool matching = DomainNameComparer.Default.Equals(x, y);
					Console.WriteLine(matching.ToYesNo());
					if (!matching) continue;
					matchingDomains.Add((x, y));
				}
			}

			if (matchingDomains.Count == 0)
			{
				Console.WriteLine(Bright.Red("No matching entries..!"));
				return;
			}

			Console.WriteLine($"Found {Bright.Green(matchingDomains.Count.ToString())} entries:");

			foreach ((string, string) tuple in matchingDomains)
			{
				Console.WriteLine(tuple.Item1);
				Console.WriteLine(tuple.Item2);
				Console.WriteLine();
			}
		}

		private static void TestFibonacci()
		{
			bool more;
			Console.Clear();
			Console.WriteLine("Testing Fibonacci number: ");

			do
			{
				Console.Write($"Type in {Bright.Green("a number")} to calculate the Fibonacci number for or {Bright.Red("ESCAPE")} key to exit. ");
				string response = Console.ReadLine();
				more = !string.IsNullOrWhiteSpace(response);
				if (more && uint.TryParse(response, out uint value)) Console.WriteLine(essentialMix.Numeric.Math.Fibonacci(value));
			}
			while (more);
		}

		private static void TestGroupAnagrams()
		{
			string[][] allWords =
			{
				null, //null
				new []{""}, //[]
				new []{"test"}, // ["test"]
				new []{"abc", "dabd", "bca", "cab", "ddba"}, //["abc", "bca", "cab"], ["dabd", "ddba"]
				new []{"abc", "cba", "bca"}, //["abc", "cba", "bca"]
				new []{"zxc", "asd", "weq", "sda", "qwe", "xcz"}, //["zxc", "xcz"], ["asd", "sda"], ["weq", "qwe"]
				new []{"yo", "act", "flop", "tac", "cat", "oy", "olfp"}, //["yo", "oy"], ["flop", "olfp"], ["act", "tac", "cat"]
				new []{"cinema", "a", "flop", "iceman", "meacyne", "lofp", "olfp"}, //["cinema", "iceman"], ["flop", "lofp", "olfp"], ["a"], ["meacyne"]
				new []{"abc", "abe", "abf", "abg"}, //["abc"], ["abe"], ["abf"], ["abg"]
			};
			int i = -1;
			bool more;
			Console.Clear();
			Console.WriteLine("Testing Group Anagrams: ");

			do
			{
				Console.Clear();
				Title("Testing group anagrams...");
				i = (i + 1) % allWords.Length;
				string[] words = allWords[i];
				Console.Write(Bright.Black("Words: "));
				
				if (words == null) Console.WriteLine("<null>");
				else if (words.Length == 0) Console.WriteLine("[]");
				else Console.WriteLine("[" + string.Join(", ", words) + "]");
				
				IReadOnlyCollection<IReadOnlyList<string>> anagrams = StringHelper.GroupAnagrams(words);
				Console.Write(Bright.Yellow("Anagrams: "));

				if (anagrams == null) Console.WriteLine("<null>");
				else if (anagrams.Count == 0) Console.WriteLine("[]");
				else Console.WriteLine(string.Join(", ", anagrams.Select(e => "[" + string.Join(", ", e) + "]")));
				
				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to move to next test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestKadaneMaximum()
		{
			int[][] allNumbers =
			{
				new int[0], //0
				new []{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, //55
				new []{-1, -2, -3, -4, -5, -6, -7, -8, -9, -10}, //-1
				new []{-10, -2, -9, -4, -8, -6, -7, -1, -3, -5}, //-1
				new []{1, 2, 3, 4, 5, 6, -20, 7, 8, 9, 10}, //35
				new []{1, 2, 3, 4, 5, 6, -22, 7, 8, 9, 10}, //34
				new []{1, 2, -4, 3, 5, -9, 8, 1, 2}, //11
				new []{3, 4, -6, 7, 8}, //16
			};
			int i = -1;
			bool more;
			Console.Clear();
			Console.WriteLine("Testing Kadane algorithm: maximum sum that can be obtained by summing up adjacent numbers");

			do
			{
				i = (i + 1) % allNumbers.Length;
				int[] numbers = allNumbers[i];
				Console.Write(Bright.Black("Numbers: "));
				
				if (numbers.Length == 0) Console.WriteLine("[]");
				else Console.WriteLine("[" + string.Join(", ", numbers) + "]");
				
				Console.WriteLine(Bright.Yellow("Sum: ") + numbers.KadaneMaximumSum());
				Console.Write($"Press {Bright.Green("[Y]")} to move to next test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestLevenshteinDistance()
		{
			(string First, string Second)[] allStrings =
			{
				(string.Empty, string.Empty), //0
				(string.Empty, "abc"), //3
				("abc", "abc"), //0
				("abc", "abx"), //1
				("abc", "abcx"), //1
				("abc", "yabcx"), //2
				("algoexpert", "algozexpert"), //1
				("abcdefghij", "1234567890"), //10
				("abcdefghij", "a234567890"), //9
				("biting", "mitten"), //4
				("cereal", "saturday"), //6
				("cereal", "saturdzz"), //7
				("abbbbbbbbb", "bbbbbbbbba"), //2
				("abc", "yabd"), //2
				("xabc", "abcx"), //2
			};

			int i = -1;
			bool more;
			Console.Clear();
			Console.WriteLine(@"Testing Levenshtein distance: minimum number of edit operations 
(insert/delete/substitute) to change first string to obtain the second string.");

			do
			{
				i = (i + 1) % allStrings.Length;
				(string first, string second) = allStrings[i];
				Console.WriteLine($"Strings: '{first}', '{second}'");
				Console.WriteLine(Bright.Yellow("Levenshtein Distance: ") + StringHelper.LevenshteinDistance(first, second));
				Console.Write($"Press {Bright.Green("[Y]")} to move to next test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestDeepestPit()
		{
			(string Label, int[] Array)[] allNumbers =
			{
				("Test case none: ", new int[0]), //-1
				("Test case 1: ", new []{0, 1, 3, -2, 0, 1, 0, -3, 2, 3}), //4
				("Test case 2: ", new []{0, 1, 3, -2, 0, 1, 0, 0, -3, 2, 3}), //3
				("Test case 3: ", new []{0, 1, 3, -2, 0, 0, 1, 0, 0, -3, 2, 3}), //3
				("Test case 4: ", new []{0, 1, 3, -2, 0, 0, 1, 0, -3, -1, 1, 2, 3}), //4
				("Test case 5: ", new []{0, 1, 3, -2, 0, 0, 1, 0, -3, -1, -1, 1, 2, 3}), //2
				("Monotonically decreasing: ", new []{0, -1, -2, -3, -4, -5, -6, -7, -8, -9}), //-1
				("Monotonically increasing: ", new []{0, 1, 2, 3}), //-1
				("All the same, zeros: ", new []{0, 0, 0, 0}), //-1
				("Extreme no pit, monotonically increasing: ", new []{-100000000, 0, 100000000}), //-1
				("Extreme depth 1 w/o pit: ", new []{100000000, 0, 0, 100000000}), //-1
				("Extreme depth 1 w pit: ", new []{100000000, 0, 100000000}), //100000000
				("Extreme depth 2 w pit: ", new []{100000000, -100000000, 0, 0, 100000000}), //100000000
				("Extreme depth 3 w pit: ", new []{100000000, -100000000, 100000000, 0, 100000000}), //200000000
				("Extreme depth 2 w false first pit: ", new []{100000000, -100000000, -100000000, 100000000, 0, 100000000}), //100000000
				("Volcano shape: ", new []{0, 1, 2, 3, 10, 100, 90, 100, 3, 2, 1, 0}), //10
				("Mountain shape: ", new []{0, 1, 2, 3, 10, 100, 3, 2, 1, 0}), //-1
				("Plateau: ", new []{0, 1, 2, 3, 10, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 3, 2, 1, 0}), //-1
			};
			int i = -1;
			bool more;
			Console.Clear();
			Console.WriteLine("Testing Deepest Pit: ");

			do
			{
				i = (i + 1) % allNumbers.Length;
				(string label, int[] numbers) = allNumbers[i];
				Console.Write(Bright.Black(label));
				
				if (numbers.Length == 0) Console.WriteLine("[]");
				else Console.WriteLine("[" + string.Join(", ", numbers) + "]");
				
				Console.WriteLine(Bright.Yellow("Deepest Pit: ") + numbers.DeepestPit());
				Console.Write($"Press {Bright.Green("[Y]")} to move to next test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestThreadQueue()
		{
			int len = RNGRandomHelper.Next(100, 1000);
			int[] values = GetRandomIntegers(len);
			int timeout = RNGRandomHelper.Next(0, 1);
			string timeoutString = timeout > 0
										? $"{timeout} minute(s)"
										: "None";
			int threads;

			if (DebugHelper.DebugMode)
			{
				// if in debug mode and LimitThreads is true, use just 1 thread for easier debugging.
				threads = LimitThreads()
							? 1
							: RNGRandomHelper.Next(TaskHelper.QueueMinimum, TaskHelper.QueueMaximum);
			}
			else
			{
				// Otherwise, use the default (Best to be TaskHelper.ProcessDefault which = Environment.ProcessorCount)
				threads = RNGRandomHelper.Next(TaskHelper.QueueMinimum, TaskHelper.QueueMaximum);
			}

			Func<int, TaskResult> exec = e =>
			{
				Console.Write(", {0}", e);
				return TaskResult.Success;
			};
			Queue<ThreadQueueMode> modes = new Queue<ThreadQueueMode>(EnumHelper<ThreadQueueMode>.GetValues());
			Stopwatch clock = new Stopwatch();

			if (threads < 1 || threads > TaskHelper.ProcessDefault) threads = TaskHelper.ProcessDefault;

			while (modes.Count > 0)
			{
				Console.Clear();
				Console.WriteLine();
				ThreadQueueMode mode = modes.Dequeue();
				Title($"Testing multi-thread queue in '{Bright.Cyan(mode.ToString())}' mode...");

				// if there is a timeout, will use a CancellationTokenSource.
				using (CancellationTokenSource cts = timeout > 0
														? new CancellationTokenSource(TimeSpan.FromMinutes(timeout))
														: null)
				{
					CancellationToken token = cts?.Token ?? CancellationToken.None;
					ProducerConsumerQueueOptions<int> options = mode == ThreadQueueMode.ThresholdTaskGroup
																	? new ProducerConsumerThresholdQueueOptions<int>(threads, exec)
																	{
																		// This can control time restriction i.e. Number of threads/tasks per second/minute etc.
																		Threshold = TimeSpan.FromSeconds(1)
																	}
																	: new ProducerConsumerQueueOptions<int>(threads, exec);
			
					// Create a generic queue producer
					using (IProducerConsumer<int> queue = ProducerConsumerQueue.Create(mode, options, token))
					{
						queue.WorkStarted += (_, _) =>
						{
							Console.WriteLine();
							Console.WriteLine($"Starting multi-thread test. mode: '{Bright.Cyan(mode.ToString())}', values: {Bright.Cyan(values.Length.ToString())}, threads: {Bright.Cyan(options.Threads.ToString())}, timeout: {Bright.Cyan(timeoutString)}...");
							if (mode == ThreadQueueMode.ThresholdTaskGroup) Console.WriteLine($"in {mode} mode, {Bright.Cyan(threads.ToString())} tasks will be issued every {Bright.Cyan(((ProducerConsumerThresholdQueueOptions<int>)options).Threshold.TotalSeconds.ToString("N0"))} second(s).");
							Console.WriteLine();
							Console.WriteLine();
							clock.Restart();
						};

						queue.WorkCompleted += (_, _) =>
						{
							long elapsed = clock.ElapsedMilliseconds;
							Console.WriteLine();
							Console.WriteLine();
							Console.WriteLine($"Finished test. mode: '{Bright.Cyan(mode.ToString())}', values: {Bright.Cyan(values.Length.ToString())}, threads: {Bright.Cyan(options.Threads.ToString())}, timeout: {Bright.Cyan(timeoutString)}, elapsed: {Bright.Cyan(elapsed.ToString())} ms.");
							Console.WriteLine();
						};

						foreach (int value in values)
						{
							queue.Enqueue(value);
						}
						
						queue.Complete();
						
						/*
						 * when the queue is being disposed, it will wait until the queued items are processed.
						 * this works when queue.WaitOnDispose is true , which it is by default.
						 * alternatively, the following can be done to wait for all items to be processed:
						 *
						 * // Important: marks the completion of queued items, no further items can be queued
						 * // after this point. the queue will not to wait for more items other than the already queued.
						 * queue.Complete();
						 * // wait for the queue to finish
						 * queue.Wait();
						 *
						 * another way to go about it, is not to call queue.Complete(); if this queue will
						 * wait indefinitely and maybe use a CancellationTokenSource.
						 *
						 * for now, the queue will wait for the items to be finished once reached the next
						 * dispose curly bracket.
						 */
					}
				}

				if (modes.Count == 0) continue;
				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to move to next test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				if (response.Key != ConsoleKey.Y) modes.Clear();
			}

			clock.Stop();
		}
		
		private static void TestSortAlgorithm()
		{
			const string ALGORITHM = nameof(IListExtension.SortInsertion);

			Action<IList<int>, int, int, IComparer<int>, bool> sortNumbers = GetAlgorithm<int>(ALGORITHM);
			Action<IList<string>, int, int, IComparer<string>, bool> sortStrings = GetAlgorithm<string>(ALGORITHM);
			Console.WriteLine($"Testing {Bright.Cyan(ALGORITHM)} algorithm: ");

			Stopwatch watch = new Stopwatch();
			IComparer<int> numbersComparer = Comparer<int>.Default;
			IComparer<string> stringComparer = StringComparer.Ordinal;
			bool more;

			do
			{
				Console.Clear();
				int[] numbers = GetRandomIntegers(RNGRandomHelper.Next(5, 20));
				string[] strings = GetRandomStrings(RNGRandomHelper.Next(3, 10)).ToArray();
				Console.WriteLine(Bright.Cyan("Numbers: ") + string.Join(", ", numbers));
				Console.WriteLine(Bright.Cyan("String: ") + string.Join(", ", strings.Select(e => e.SingleQuote())));

				Console.Write("Numbers");
				watch.Restart();
				sortNumbers(numbers, 0, -1, numbersComparer, false);
				long numericResults = watch.ElapsedMilliseconds;
				watch.Stop();
				Console.WriteLine($" => {Bright.Green(numericResults.ToString())}");
				Console.WriteLine("Result: " + string.Join(", ", numbers));
				Console.WriteLine();

				Console.Write("Strings");
				watch.Restart();
				sortStrings(strings, 0, -1, stringComparer, false);
				long stringResults = watch.ElapsedMilliseconds;
				watch.Stop();
				Console.WriteLine($" => {Bright.Green(stringResults.ToString())}");
				Console.WriteLine("Result: " + string.Join(", ", strings.Select(e => e.SingleQuote())));
				Console.WriteLine();

				Console.WriteLine(Bright.Yellow("Finished"));
				Console.WriteLine();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestSortAlgorithms()
		{
			const int RESULT_COUNT = 10;

			Title("Testing Sort Algorithms...");

			Stopwatch watch = new Stopwatch();
			IComparer<int> numbersComparer = Comparer<int>.Default;
			IComparer<string> stringComparer = StringComparer.Ordinal;
			IDictionary<string, long> numericResults = new Dictionary<string, long>();
			IDictionary<string, long> stringResults = new Dictionary<string, long>();
			string sectionSeparator = Bright.Magenta(new string('*', 80));
			bool more;
			int tests = 0;
			int[] numbers = GetRandomIntegers(START);
			string[] strings = GetRandomStrings(START).ToArray();

			do
			{
				Console.Clear();

				if (tests == 0)
				{
					Console.WriteLine(Bright.Cyan("Numbers: ") + string.Join(", ", numbers));
					Console.WriteLine(Bright.Cyan("String: ") + string.Join(", ", strings.Select(e => e.SingleQuote())));
					CompilationHint();
				}

				foreach (string algorithm in __sortAlgorithms)
				{
					GC.Collect();
					Action<IList<int>, int, int, IComparer<int>, bool> sortNumbers = GetAlgorithm<int>(algorithm);
					Action<IList<string>, int, int, IComparer<string>, bool> sortStrings = GetAlgorithm<string>(algorithm);
					Console.WriteLine(sectionSeparator);
					Console.WriteLine($"Testing {Bright.Cyan(algorithm)} algorithm: ");

					Console.Write("Numbers");
					int[] ints = (int[])numbers.Clone();
					watch.Restart();
					sortNumbers(ints, 0, -1, numbersComparer, false);
					numericResults[algorithm] = watch.ElapsedTicks;
					Console.WriteLine($" => {Bright.Green(numericResults[algorithm].ToString())}");
					if (tests == 0) Console.WriteLine("Result: " + string.Join(", ", ints));

					Console.Write("Strings");

					string[] str = (string[])strings.Clone();
					watch.Restart();
					sortStrings(str, 0, -1, stringComparer, false);
					stringResults[algorithm] = watch.ElapsedTicks;
					Console.WriteLine($" => {Bright.Green(stringResults[algorithm].ToString())}");
					if (tests == 0) Console.WriteLine("Result: " + string.Join(", ", str.Select(e => e.SingleQuote())));
					Console.WriteLine();
				}

				Console.WriteLine(sectionSeparator);
				Console.WriteLine(Bright.Yellow("Finished"));
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Fastest {RESULT_COUNT} numeric sort:"));
			
				foreach (KeyValuePair<string, long> pair in numericResults
															.OrderBy(e => e.Value)
															.Take(RESULT_COUNT))
				{
					Console.WriteLine($"{pair.Key} {pair.Value}");
				}

				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Fastest {RESULT_COUNT} string sort:"));

				foreach (KeyValuePair<string, long> pair in stringResults
															.OrderBy(e => e.Value)
															.Take(RESULT_COUNT))
				{
					Console.WriteLine($"{pair.Key} {pair.Value}");
				}

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 2) continue;

				if (tests > 0)
				{
					Console.Write($"Would you like to increase the array size? {Bright.Green("[Y]")} or {Dim("any other key")} to exit. ");
					response = Console.ReadKey(true);
					Console.WriteLine();
					if (response.Key != ConsoleKey.Y) continue;
				}

				switch (tests)
				{
					case 0:
						numbers = GetRandomIntegers(SMALL);
						strings = GetRandomStrings(SMALL).ToArray();
						break;
					case 1:
						numbers = GetRandomIntegers(MEDIUM);
						strings = GetRandomStrings(MEDIUM).ToArray();
						break;
					case 2:
						numbers = GetRandomIntegers(HEAVY);
						strings = GetRandomStrings(HEAVY).ToArray();
						break;
				}

				tests++;
			}
			while (more);
		}

		private static void TestLinkedQueue()
		{
			Title("Testing LinkedQueue...");
			
			int len = RNGRandomHelper.Next(5, 20);
			int[] values = GetRandomIntegers(len);
			Console.WriteLine("Array: " + string.Join(", ", values));

			Console.WriteLine("As Queue:");
			LinkedQueue<int> queue = new LinkedQueue<int>(DequeuePriority.FIFO);
	
			foreach (int value in values)
			{
				queue.Enqueue(value);
			}
	
			while (queue.Count > 0)
			{
				Console.WriteLine(queue.Dequeue());
			}

			Console.WriteLine("As Stack:");
			queue = new LinkedQueue<int>(DequeuePriority.LIFO);

			foreach (int value in values)
			{
				queue.Enqueue(value);
			}

			while (queue.Count > 0)
			{
				Console.WriteLine(queue.Dequeue());
			}
		}

		private static void TestMinMaxQueue()
		{
			Title("Testing MinMaxQueue...");

			int len = RNGRandomHelper.Next(5, 20);
			int[] values = GetRandomIntegers(len);
			Console.WriteLine("Array: " + string.Join(", ", values));

			Console.WriteLine("As Queue:");
			MinMaxQueue<int> queue = new MinMaxQueue<int>();

			foreach (int value in values)
			{
				queue.Enqueue(value);
				Console.WriteLine($"Adding Value: {value}, Min: {queue.Minimum}, Max: {queue.Maximum}");
			}
	
			Console.WriteLine();

			while (queue.Count > 0)
			{
				Console.WriteLine($"Dequeue Value: {queue.Dequeue()}, Min: {queue.Minimum}, Max: {queue.Maximum}");
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("As Stack:");
			
			MinMaxStack<int> stack = new MinMaxStack<int>();

			foreach (int value in values)
			{
				stack.Push(value);
				Console.WriteLine($"Adding Value: {value}, Min: {stack.Minimum}, Max: {stack.Maximum}");
			}

			Console.WriteLine();

			while (stack.Count > 0)
			{
				Console.WriteLine($"Dequeue Value: {stack.Pop()}, Min: {stack.Minimum}, Max: {stack.Maximum}");
			}
		}

		private static void TestSinglyLinkedList()
		{
			bool more;
			int tests = 0;
			Stopwatch clock = new Stopwatch();
			int[] values = GetRandomIntegers(true, START);
			SinglyLinkedList<int> list = new SinglyLinkedList<int>();

			do
			{
				Console.Clear();
				Title("Testing SingleLinkedList...");
				CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");
				Console.WriteLine("Test adding...");

				list.Clear();
				int count = list.Count;
				Debug.Assert(count == 0, "Values are not cleared correctly!");
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				clock.Restart();

				foreach (int v in values)
				{
					list.AddLast(v);
					count++;
				}

				Console.WriteLine($"Added {count} items of {values.Length} in {clock.ElapsedMilliseconds} ms.");

				if (list.Count != values.Length)
				{
					Console.WriteLine(Bright.Red("Something went wrong, Count isn't right...!"));
					return;
				}

				Console.WriteLine(Bright.Yellow("Test find a random value..."));
				int x = IListExtension.PickRandom(values);
				SinglyLinkedListNode<int> node = list.Find(x);

				if (node == null)
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				int value = IListExtension.PickRandom(values);
				Console.WriteLine($"Found. Now will add {Bright.Cyan().Underline(value.ToString())} after {Bright.Cyan().Underline(x.ToString())}...");
				list.AddAfter(node, value);
				Console.WriteLine("Node's next: " + node.Next.Value);
				list.Remove(node.Next);

				Console.WriteLine($"Test adding {Bright.Cyan().Underline(value.ToString())} before {Bright.Cyan().Underline(x.ToString())}...");
				SinglyLinkedListNode<int> previous = list.AddBefore(node, value);
				list.Remove(previous);
	
				Console.WriteLine($"Test adding {Bright.Cyan().Underline(value.ToString())} to the beginning of the list...");
				list.AddFirst(value);
				list.RemoveFirst();

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				int missed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (list.Contains(v))
					{
						found++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Find missed a value: {v} :((")
										: Bright.Red("FIND MISSED A LOT :(("));
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Found {found} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				missed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (list.Remove(v))
					{
						removed++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Remove missed a value: {v} :((")
										: Bright.Red("REMOVE MISSED A LOT. :(("));
					Console.WriteLine("Does it contain the value? " + list.Contains(v).ToYesNo());
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Removed {removed} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				tests++;
			}
			while (more);

			clock.Stop();
		}

		private static void TestLinkedList()
		{
			bool more;
			int tests = 0;
			Stopwatch clock = new Stopwatch();
			int[] values = GetRandomIntegers(true, START);
			LinkedList<int> list = new LinkedList<int>();

			do
			{
				Console.Clear();
				Title("Testing LinkedList...");
				CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");
				Console.WriteLine("Test adding...");

				list.Clear();
				int count = list.Count;
				Debug.Assert(count == 0, "Values are not cleared correctly!");
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				clock.Restart();

				foreach (int v in values)
				{
					list.AddLast(v);
					count++;
				}

				Console.WriteLine($"Added {count} items of {values.Length} in {clock.ElapsedMilliseconds} ms.");

				if (list.Count != values.Length)
				{
					Console.WriteLine(Bright.Red("Something went wrong, Count isn't right...!"));
					return;
				}

				Console.WriteLine(Bright.Yellow("Test find a random value..."));
				int x = IListExtension.PickRandom(values);
				LinkedListNode<int> node = list.Find(x);

				if (node == null)
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				int value = IListExtension.PickRandom(values);
				Console.WriteLine($"Found. Now will add {Bright.Cyan().Underline(value.ToString())} after {Bright.Cyan().Underline(x.ToString())}...");
				list.AddAfter(node, value);
				Console.WriteLine("Node's next: " + node.Next?.Value);

				Console.WriteLine($"Test adding {Bright.Cyan().Underline(value.ToString())} before {Bright.Cyan().Underline(x.ToString())}...");
				list.AddBefore(node, value);
				Console.WriteLine("Node's previous: " + node.Previous?.Value);
				
				Console.WriteLine($"Test adding {Bright.Cyan().Underline(value.ToString())} to the beginning of the list...");
				list.AddFirst(value);

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				int missed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (list.Contains(v))
					{
						found++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Find missed a value: {v} :((")
										: Bright.Red("FIND MISSED A LOT :(("));
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Found {found} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				missed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (list.Remove(v))
					{
						removed++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Remove missed a value: {v} :((")
										: Bright.Red("REMOVE MISSED A LOT. :(("));
					Console.WriteLine("Does it contain the value? " + list.Contains(v).ToYesNo());
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Removed {removed} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				tests++;
			}
			while (more);

			clock.Stop();
		}

		private static void TestDeque()
		{
			bool more;
			int tests = 0;
			Stopwatch clock = new Stopwatch();
			int[] values = GetRandomIntegers(true, START);
			Deque<int> deque = new Deque<int>();

			do
			{
				Console.Clear();
				Title("Testing Deque...");
				CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");
				Console.WriteLine("Test queue functionality...");

				Console.Write($"Would you like to print the results? {Bright.Green("[Y]")} or {Dim("any other key")}: ");
				bool print = Console.ReadKey(true).Key == ConsoleKey.Y;
				Console.WriteLine();

				// Queue test
				Title("Testing Deque as a Queue...");
				DoTheTest(deque, values, deque.Enqueue, deque.Dequeue, print, clock);
				Title("End testing Deque as a Queue...");
				ConsoleHelper.Pause();

				// Stack test
				Title("Testing Deque as a Stack...");
				DoTheTest(deque, values, deque.Push, deque.Pop, print, clock);
				Title("End testing Deque as a Stack...");

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				tests++;
			}
			while (more);

			clock.Stop();

			static void DoTheTest(Deque<int> deque, int[] values, Action<int> add, Func<int> remove, bool print, Stopwatch clock)
			{
				deque.Clear();
				int count = deque.Count;
				Debug.Assert(count == 0, "Values are not cleared correctly!");
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				clock.Restart();

				foreach (int v in values)
				{
					add(v);
					count++;
				}

				Console.WriteLine($"Added {count} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");

				if (deque.Count != values.Length)
				{
					Console.WriteLine(Bright.Red("Something went wrong, Count isn't right...!"));
					return;
				}

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				int missed = 0;
				count = deque.Count / 4;
				clock.Restart();

				// will just test for items not more than MAX_SEARCH
				for (int i = 0; i < count; i++)
				{
					int v = values[i];

					if (deque.Contains(v))
					{
						found++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Find missed a value: {v} :((")
										: Bright.Red("FIND MISSED A LOT :(("));
					if (missed > 3) return;
					//return;
				}

				Console.WriteLine($"Found {found} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
		
				int removed = 0;
				count = deque.Count;
				clock.Restart();

				if (print)
				{
					while (deque.Count > 0 && count > 0)
					{
						Console.Write(remove());
						count--;
						removed++;
						if (deque.Count > 0) Console.Write(", ");
					}
				}
				else
				{
					while (deque.Count > 0 && count > 0)
					{
						remove();
						count--;
						removed++;
					}
				}

				Debug.Assert(count == 0 && deque.Count == 0, $"Values are not cleared correctly! {count} != {deque.Count}.");
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"Removed {removed} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");
			}
		}

		private static void TestLinkedDeque()
		{
			bool more;
			int tests = 0;
			Stopwatch clock = new Stopwatch();
			int[] values = GetRandomIntegers(true, START);
			LinkedDeque<int> deque = new LinkedDeque<int>();

			do
			{
				Console.Clear();
				Title("Testing Deque...");
				CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");
				Console.WriteLine("Test queue functionality...");

				Console.Write($"Would you like to print the results? {Bright.Green("[Y]")} or {Dim("any other key")}: ");
				bool print = Console.ReadKey(true).Key == ConsoleKey.Y;
				Console.WriteLine();

				// Queue test
				Title("Testing Deque as a Queue...");
				DoTheTest(deque, values, deque.Enqueue, deque.Dequeue, print, clock);
				Title("End testing Deque as a Queue...");
				ConsoleHelper.Pause();

				// Stack test
				Title("Testing Deque as a Stack...");
				DoTheTest(deque, values, deque.Push, deque.Pop, print, clock);
				Title("End testing Deque as a Stack...");

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				tests++;
			}
			while (more);

			clock.Stop();

			static void DoTheTest(LinkedDeque<int> deque, int[] values, Action<int> add, Func<int> remove, bool print, Stopwatch clock)
			{
				deque.Clear();
				int count = deque.Count;
				Debug.Assert(count == 0, "Values are not cleared correctly!");
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				clock.Restart();

				foreach (int v in values)
				{
					add(v);
					count++;
				}

				Console.WriteLine($"Added {count} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");

				if (deque.Count != values.Length)
				{
					Console.WriteLine(Bright.Red("Something went wrong, Count isn't right...!"));
					return;
				}

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				int missed = 0;
				count = deque.Count / 4;
				clock.Restart();

				// will just test for items not more than MAX_SEARCH
				for (int i = 0; i < count; i++)
				{
					int v = values[i];

					if (deque.Contains(v))
					{
						found++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Find missed a value: {v} :((")
										: Bright.Red("FIND MISSED A LOT :(("));
					if (missed > 3) return;
					//return;
				}

				Console.WriteLine($"Found {found} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				count = deque.Count;
				clock.Restart();

				if (print)
				{
					while (deque.Count > 0 && count > 0)
					{
						Console.Write(remove());
						count--;
						removed++;
						if (deque.Count > 0) Console.Write(", ");
					}
				}
				else
				{
					while (deque.Count > 0 && count > 0)
					{
						remove();
						count--;
						removed++;
					}
				}

				Debug.Assert(count == 0 && deque.Count == 0, $"Values are not cleared correctly! {count} != {deque.Count}.");
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"Removed {removed} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");
			}
		}

		private static void TestBinaryTreeFromTraversal()
		{
			const string TREE_DATA_LEVEL = "FCIADGJBEHK";
			const string TREE_DATA_PRE = "FCABDEIGHJK";
			const string TREE_DATA_IN = "ABCDEFGHIJK";
			const string TREE_DATA_POST = "BAEDCHGKJIF";
			const int NUM_TESTS = 7;

			BinarySearchTree<char> tree = new BinarySearchTree<char>();

			bool more;
			int i = 0;

			do
			{
				Console.Clear();
				Title("Testing BinaryTree from traversal values...");

				switch (i)
				{
					case 0:
						Console.WriteLine($@"Data from {Bright.Cyan("LevelOrder")} traversal:
{TREE_DATA_LEVEL}");
						tree.FromLevelOrder(TREE_DATA_LEVEL);
						break;
					case 1:
						Console.WriteLine($@"Data from {Bright.Cyan("PreOrder")} traversal:
{TREE_DATA_PRE}");
						tree.FromPreOrder(TREE_DATA_PRE);
						break;
					case 2:
						Console.WriteLine($@"Data from {Bright.Cyan("InOrder")} traversal:
{TREE_DATA_IN}");
						tree.FromInOrder(TREE_DATA_IN);
						break;
					case 3:
						Console.WriteLine($@"Data from {Bright.Cyan("PostOrder")} traversal:
{TREE_DATA_POST}");
						tree.FromPostOrder(TREE_DATA_POST);
						break;
					case 4:
						Console.WriteLine($@"Data from {Bright.Cyan("InOrder")} and {Bright.Cyan("LevelOrder")} traversals:
{TREE_DATA_IN}
{TREE_DATA_LEVEL}");
						tree.FromInOrderAndLevelOrder(TREE_DATA_IN, TREE_DATA_LEVEL);
						break;
					case 5:
						Console.WriteLine($@"Data from {Bright.Cyan("InOrder")} and {Bright.Cyan("PreOrder")} traversals:
{TREE_DATA_IN}
{TREE_DATA_PRE}");
						tree.FromInOrderAndPreOrder(TREE_DATA_IN, TREE_DATA_PRE);
						break;
					case 6:
						Console.WriteLine($@"Data from {Bright.Cyan("InOrder")} and {Bright.Cyan("PostOrder")} traversals:
{TREE_DATA_IN}
{TREE_DATA_POST}");
						tree.FromInOrderAndPostOrder(TREE_DATA_IN, TREE_DATA_POST);
						break;
				}

				tree.PrintWithProps();
				i++;

				if (i >= NUM_TESTS)
				{
					more = false;
					continue;
				}

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to move to next test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestBinarySearchTreeAdd()
		{
			bool more;
			BinarySearchTree<int> tree = new BinarySearchTree<int>();

			do
			{
				Console.Clear();
				Title("Testing BinarySearchTree.Add()...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();

				foreach (int v in values)
				{
					tree.Add(v);
					//tree.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestBinarySearchTreeRemove()
		{
			bool more;
			BinarySearchTree<int> tree = new BinarySearchTree<int>();

			do
			{
				Console.Clear();
				Title("Testing BinarySearchTree.Remove()...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();
				tree.Add(values);
				Debug.Assert(tree.Count == values.Length, $"Values are not added correctly! {values.Length} != {tree.Count}.");
				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine("Test finding a random value...");
				int value = IListExtension.PickRandom(values);
				Console.WriteLine($"will look for {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Contains(value))
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				Console.WriteLine("Found.");
				
				int found = 0;
				Console.WriteLine("Test finding all values...");

				foreach (int v in values)
				{
					if (tree.Contains(v))
					{
						found++;
						continue;
					}

					Console.WriteLine(Bright.Red($"Find missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
				}
				Console.WriteLine($"Found {found} of {values.Length} items.");

				Console.WriteLine();
				Console.WriteLine("Test removing a random value...");
				value = IListExtension.PickRandom(values);
				Console.WriteLine($"will remove {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Remove(value))
				{
					Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
					return;
				}

				tree.PrintWithProps();

				int removed = 1;
				Console.WriteLine();
				Console.WriteLine("Test removing all values...");

				foreach (int v in values)
				{
					if (v == value) continue;

					if (tree.Remove(v))
					{
						removed++;
						Debug.Assert(values.Length - removed == tree.Count, $"Values are not removed correctly! {values.Length - removed} != {tree.Count}.");
						continue;
					}

					Console.WriteLine(Bright.Red($"Remove missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
				}

				Console.WriteLine("OK");
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestBinarySearchTreeBalance()
		{
			bool more;
			BinarySearchTree<int> tree = new BinarySearchTree<int>();

			do
			{
				Console.Clear();
				Title("Testing BinarySearchTree.Balance()...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();
				tree.Add(values);
				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine(Bright.Red("Test removing..."));
				int value = IListExtension.PickRandom(values);
				Console.WriteLine($"will remove {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Remove(value))
				{
					Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
					return;
				}

				tree.PrintWithProps();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestBinaryTreeFindClosest()
		{
			bool more;
			Console.Clear();
			Title("Testing BinaryTree FindClosest...");

			BinarySearchTree<int> tree = new BinarySearchTree<int>();
			int len = RNGRandomHelper.Next(1, 50);
			int[] values = GetRandomIntegers(true, len);
			int min = values.Min();
			int max = values.Max();
			tree.Clear();
			tree.Add(values);
			tree.Print();

			do
			{
				int value = RNGRandomHelper.Next(min, max);
				Console.WriteLine($"Closest value to {Yellow(value.ToString())} => {tree.FindClosestValue(value, -1)} ");

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestBinaryTreeBranchSums()
		{
			bool more;
			BinarySearchTree<int> tree = new BinarySearchTree<int>();

			do
			{
				Console.Clear();
				Title("Testing BinaryTree BranchSums...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				tree.Clear();
				tree.Add(values);
				tree.Print();
				Console.WriteLine(Bright.Black("Branch Sums: ") + string.Join(", ", tree.BranchSums()));

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestBinaryTreeInvert()
		{
			bool more;
			BinarySearchTree<int> tree = new BinarySearchTree<int>();

			do
			{
				Console.Clear();
				Title("Testing BinaryTree Invert...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				tree.Clear();
				tree.Add(values);
				tree.Print();
				Console.WriteLine(Bright.Yellow("Inverted: "));

				tree.Invert();
				tree.Print();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestAVLTreeAdd()
		{
			bool more;
			AVLTree<int> tree = new AVLTree<int>();

			do
			{
				Console.Clear();
				Title("Testing AVLTree.Add()...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();

				foreach (int v in values)
				{
					tree.Add(v);
					//tree.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestAVLTreeRemove()
		{
			bool more;
			AVLTree<int> tree = new AVLTree<int>();

			do
			{
				Console.Clear();
				Title("Testing AVLTree.Remove()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();
				tree.Add(values);
				Debug.Assert(tree.Count == values.Length, $"Values are not added correctly! {values.Length} != {tree.Count}.");
				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine("Test finding a random value...");
				int value = IListExtension.PickRandom(values);
				Console.WriteLine($"will look for {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Contains(value))
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				Console.WriteLine("Found.");

				int found = 0;
				Console.WriteLine("Test finding all values...");

				foreach (int v in values)
				{
					if (tree.Contains(v))
					{
						found++;
						continue;
					}

					Console.WriteLine(Bright.Red($"Find missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
				}
				Console.WriteLine($"Found {found} of {values.Length} items.");

				Console.WriteLine(Bright.Red("Test removing..."));
				value = IListExtension.PickRandom(values);
				Console.WriteLine($"will remove {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Remove(value))
				{
					Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
					return;
				}

				tree.PrintWithProps();

				int removed = 1;
				Console.WriteLine();
				Console.WriteLine("Test removing all values...");

				foreach (int v in values)
				{
					if (v == value) continue;

					if (tree.Remove(v))
					{
						removed++;
						Debug.Assert(values.Length - removed == tree.Count, $"Values are not removed correctly! {values.Length - removed} != {tree.Count}.");
						continue;
					}
					Console.WriteLine(Bright.Red($"Remove missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
				}

				Console.WriteLine("OK");
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestRedBlackTreeAdd()
		{
			bool more;
			RedBlackTree<int> tree = new RedBlackTree<int>();

			do
			{
				Console.Clear();
				Title("Testing RedBlackTree.Add()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();

				foreach (int v in values)
				{
					tree.Add(v);
					//tree.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestRedBlackTreeRemove()
		{
			bool more;
			RedBlackTree<int> tree = new RedBlackTree<int>();

			do
			{
				Console.Clear();
				Title("Testing RedBlackTree.Remove()...");
				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine(Bright.Green("Test adding..."));
				tree.Clear();
				tree.Add(values);
				Debug.Assert(tree.Count == values.Length, $"Values are not added correctly! {values.Length} != {tree.Count}.");
				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine("Test finding a random value...");

				int value = IListExtension.PickRandom(values);
				Console.WriteLine($"will look for {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Contains(value))
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				Console.WriteLine("Found.");

				int found = 0;
				Console.WriteLine("Test finding all values...");

				foreach (int v in values)
				{
					if (tree.Contains(v))
					{
						found++;
						continue;
					}

					Console.WriteLine(Bright.Red($"Find missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
				}
				Console.WriteLine($"Found {found} of {values.Length} items.");

				Console.WriteLine();
				Console.WriteLine(Bright.Red("Test removing..."));
				value = IListExtension.PickRandom(values);
				Console.WriteLine($"will remove {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Remove(value))
				{
					Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
					return;
				}

				tree.PrintWithProps();

				int removed = 1;
				Console.WriteLine();
				Console.WriteLine("Test removing all values...");

				foreach (int v in values)
				{
					if (v == value) continue;

					if (tree.Remove(v))
					{
						removed++;
						Debug.Assert(values.Length - removed == tree.Count, $"Values are not removed correctly! {values.Length - removed} != {tree.Count}.");
						continue;
					}
					Console.WriteLine(Bright.Red($"Remove missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
				}

				Console.WriteLine("OK");
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
		}

		private static void TestAllBinaryTrees()
		{
			bool more;
			BinarySearchTree<int> binarySearchTree = new BinarySearchTree<int>();
			AVLTree<int> avlTree = new AVLTree<int>();
			RedBlackTree<int> redBlackTree = new RedBlackTree<int>();

			do
			{
				Console.Clear();
				Title("Testing all BinaryTrees...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				DoTheTest(binarySearchTree, values);

				DoTheTest(avlTree, values);

				DoTheTest(redBlackTree, values);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode>(LinkedBinaryTree<TNode, int> tree, int[] array)
				where TNode : LinkedBinaryNode<TNode, int>
			{
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Testing {tree.GetType().Name}..."));
				tree.Clear();
				tree.Add(array);

				Console.WriteLine(Bright.Black("InOrder: ") + string.Join(", ", tree));
				tree.PrintWithProps();

				Console.WriteLine(Bright.Red("Test removing..."));
				int value = IListExtension.PickRandom(array);
				Console.WriteLine($"will remove {Bright.Cyan().Underline(value.ToString())}.");

				if (!tree.Remove(value))
				{
					Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
					return;
				}

				tree.PrintWithProps();
			}
		}

		private static void TestAllBinaryTreesFunctionality()
		{
			bool more;
			BinarySearchTree<int> binarySearchTree = new BinarySearchTree<int>();
			AVLTree<int> avlTree = new AVLTree<int>();
			RedBlackTree<int> redBlackTree = new RedBlackTree<int>();
			int[] values = GetRandomIntegers(true, 30);

			do
			{
				Console.Clear();
				Title("Testing all BinaryTrees performance...");

				DoTheTest(binarySearchTree, values);
				ConsoleHelper.Pause();

				DoTheTest(avlTree, values);
				ConsoleHelper.Pause();

				DoTheTest(redBlackTree, values);
				ConsoleHelper.Pause();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode>(LinkedBinaryTree<TNode, int> tree, int[] values)
				where TNode : LinkedBinaryNode<TNode, int>
			{
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Testing {tree.GetType().Name}..."));
				tree.Clear();
				Debug.Assert(tree.Count == 0, "Values are not cleared correctly!");

				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				Console.WriteLine();
				Console.WriteLine($"Array: {string.Join(", ", values)}");
				
				tree.Add(values);
				Console.WriteLine($"Added {tree.Count} of {values.Length} items.");
				tree.PrintProps();

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				int missed = 0;

				foreach (int v in values)
				{
					if (tree.Contains(v))
					{
						found++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Find missed a value: {v} :((")
										: Bright.Red("FIND MISSED A LOT :(("));
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Found {found} of {values.Length} items.");

				tree.Print();

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				missed = 0;

				foreach (int v in values)
				{
					Console.WriteLine($"Removing {v}:");

					if (tree.Remove(v))
					{
						removed++;
						tree.Print();
						Console.WriteLine("Tree Count = " + tree.Count);
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Remove missed a value: {v} :((")
										: Bright.Red("REMOVE MISSED A LOT. :(("));
					Console.WriteLine("Does it contain the value? " + tree.Contains(v).ToYesNo());
					tree.Print();
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Removed {removed} of {values.Length} items.");
			}
		}

		private static void TestAllBinaryTreesPerformance()
		{
			bool more;
			int tests = 0;
			Stopwatch clock = new Stopwatch();
			BinarySearchTree<int> binarySearchTree = new BinarySearchTree<int>();
			AVLTree<int> avlTree = new AVLTree<int>();
			RedBlackTree<int> redBlackTree = new RedBlackTree<int>();
			int[] values = GetRandomIntegers(true, START);

			do
			{
				Console.Clear();
				Title("Testing all BinaryTrees performance...");
				CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");

				DoTheTest(binarySearchTree, values, clock);
				clock.Stop();

				DoTheTest(avlTree, values, clock);
				clock.Stop();

				DoTheTest(redBlackTree, values, clock);
				clock.Stop();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				tests++;
			}
			while (more);

			clock.Stop();

			static void DoTheTest<TNode>(LinkedBinaryTree<TNode, int> tree, int[] values, Stopwatch clock)
				where TNode : LinkedBinaryNode<TNode, int>
			{
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Testing {tree.GetType().Name}..."));
				tree.Clear();
				Debug.Assert(tree.Count == 0, "Values are not cleared correctly!");
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");

				clock.Restart();
				tree.Add(values);
				Console.WriteLine($"Added {tree.Count} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");
				tree.PrintProps();

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				int missed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (tree.Contains(v))
					{
						found++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Find missed a value: {v} :((")
										: Bright.Red("FIND MISSED A LOT :(("));
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Found {found} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				missed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (tree.Remove(v))
					{
						removed++;
						continue;
					}

					missed++;
					Console.WriteLine(missed <= 3
										? Bright.Red($"Remove missed a value: {v} :((")
										: Bright.Red("REMOVE MISSED A LOT. :(("));
					if (missed > 3) return;
					//return;
				}
				Console.WriteLine($"Removed {removed} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");
			}
		}

		private static void TestSortedSetPerformance()
		{
			bool more;
			int tests = 0;
			Stopwatch clock = new Stopwatch();
			// this is a RedBlackTree implementation by Microsoft, just testing it.
			SortedSet<int> sortedSet = new SortedSet<int>();
			int[] values = GetRandomIntegers(true, START);

			do
			{
				Console.Clear();
				Title("Testing SortedSet performance...");
				CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");

				DoTheTest(sortedSet, values, clock);
				clock.Stop();

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				tests++;
			}
			while (more);
			
			clock.Stop();

			static void DoTheTest<T>(SortedSet<T> sortedSet, T[] values, Stopwatch clock)
			{
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Testing {sortedSet.GetType().Name}..."));
				sortedSet.Clear();

				int count = sortedSet.Count;
				Debug.Assert(count == 0, "Values are not cleared correctly!");
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				clock.Restart();

				foreach (T v in values)
				{
					sortedSet.Add(v);
					count++;
					Debug.Assert(count == sortedSet.Count, $"Values are not added correctly! {count} != {sortedSet.Count}.");
				}

				Console.WriteLine($"Added {count} items of {values.Length} in {clock.ElapsedMilliseconds} ms.");
				Console.WriteLine();
				Console.WriteLine($"{Yellow("Count:")} {Underline(sortedSet.Count.ToString())}.");
				Console.WriteLine($"{Yellow("Minimum:")} {sortedSet.Min} {Yellow("Maximum:")} {sortedSet.Max}");

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				clock.Restart();

				foreach (T v in values)
				{
					if (sortedSet.Contains(v))
					{
						found++;
						continue;
					}

					Console.WriteLine(Bright.Red($"Find missed a value: {v} :(("));
					ConsoleHelper.Pause();
					//return;
				}
				Console.WriteLine($"Found {found} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				clock.Restart();

				foreach (T v in values)
				{
					if (sortedSet.Remove(v))
					{
						removed++;
						Debug.Assert(count - removed == sortedSet.Count, $"Values are not removed correctly! {count} != {sortedSet.Count}.");
						continue;
					}
					
					Console.WriteLine(Bright.Red($"Remove missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}
				Console.WriteLine($"Removed {removed} of {count} items in {clock.ElapsedMilliseconds} ms.");
			}
		}

		private static void TestTreeEquality()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing tree equality...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(true, len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				Console.WriteLine();
				Console.WriteLine(Bright.Black("Testing BinarySearchTree: ") + string.Join(", ", values));
				Console.WriteLine();
				LinkedBinaryTree<int> tree1 = new BinarySearchTree<int>();
				LinkedBinaryTree<int> tree2 = new BinarySearchTree<int>();
				DoTheTest(tree1, tree2, values);

				Console.WriteLine();
				Console.WriteLine(Bright.Black("Testing BinarySearchTree and AVLTree: ") + string.Join(", ", values));
				Console.WriteLine();
				tree1.Clear();
				tree2 = new AVLTree<int>();
				DoTheTest(tree1, tree2, values);

				Console.WriteLine();
				Console.WriteLine(Bright.Black("Testing AVLTree: ") + string.Join(", ", values));
				Console.WriteLine();
				tree1 = new AVLTree<int>();
				tree2 = new AVLTree<int>();
				DoTheTest(tree1, tree2, values);

				Console.WriteLine();
				Console.WriteLine(Bright.Black("Testing RedBlackTree: ") + string.Join(", ", values));
				Console.WriteLine();
				RedBlackTree<int> rbTree1 = new RedBlackTree<int>();
				RedBlackTree<int> rbTree2 = new RedBlackTree<int>();
				DoTheTest(rbTree1, rbTree2, values);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode>(LinkedBinaryTree<TNode, int> tree1, LinkedBinaryTree<TNode, int> tree2, int[] array)
				where TNode : LinkedBinaryNode<TNode, int>
			{
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Testing {tree1.GetType().Name} and {tree1.GetType().Name}..."));
				tree1.Add(array);
				tree2.Add(array);

				Console.WriteLine(Bright.Black("InOrder1: ") + string.Join(", ", tree1));
				Console.WriteLine(Bright.Black("InOrder2: ") + string.Join(", ", tree2));
				tree1.PrintWithProps();
				tree2.PrintWithProps();
				Console.WriteLine($"tree1 == tree2? {tree1.Equals(tree2).ToYesNo()}");
			}
		}

		private static void TestTrie()
		{
			const int MAX_LIST = 100;

			bool more;
			Trie<char> trie = new Trie<char>(CharComparer.InvariantCultureIgnoreCase);
			ISet<string> values = new HashSet<string>();

			do
			{
				Console.Clear();
				Title("Testing Trie...");
				if (values.Count == 0) AddWords(trie, values);
				Console.WriteLine(Bright.Black("Words list: ") + string.Join(", ", values));

				string word = values.PickRandom();
				DoTheTest(trie, word, values);
				
				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || values.Count >= MAX_LIST) continue;

				Console.Write($"Would you like to add more words? {Bright.Green("[Y]")} / {Dim("any key")} ");
				response = Console.ReadKey(true);
				Console.WriteLine();
				if (response.Key != ConsoleKey.Y) continue;
				Console.WriteLine();
				AddWords(trie, values);
			}
			while (more);

			static void AddWords(Trie<char> trie, ISet<string> set)
			{
				int len = RNGRandomHelper.Next(10, 20);
				Console.WriteLine(Bright.Green($"Generating {len} words: "));
				ICollection<string> newValues = GetRandomStrings(true, len);

				foreach (string value in newValues)
				{
					if (!set.Add(value)) continue;
					trie.Add(value);
				}
			}

			static void DoTheTest(Trie<char> trie, string token, ISet<string> values)
			{
				Console.WriteLine($"Test find '{Bright.Cyan().Underline(token)}'...");

				if (!trie.Contains(token))
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}
				
				Console.WriteLine(Bright.Green("Found...!") + " Let's try all caps...");

				if (!trie.Contains(token.ToUpperInvariant()))
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				Console.WriteLine(Bright.Green("Found...!") + " Let's try words with a common prefix...");

				string prefix = token;

				if (prefix.Length > 1)
				{
					Match match = Regex.Match(prefix, @"^([\w\-]+)");
					prefix = !match.Success || match.Value.Length == prefix.Length
								? prefix.Left(prefix.Length / 2)
								: match.Value;
				}

				Console.WriteLine($"Prefix: '{Bright.Cyan().Underline(prefix)}'");
				int results = 0;

				foreach (IEnumerable<char> enumerable in trie.Find(prefix))
				{
					Console.WriteLine($"{++results}: " + string.Concat(enumerable));
				}

				if (results == 0)
				{
					Console.WriteLine(Bright.Red("Didn't find a shit...!"));
					return;
				}

				int tries = 3;

				while (prefix.Length > 1 && results < 2 && --tries > 0)
				{
					results = 0;
					prefix = prefix.Left(prefix.Length / 2);
					Console.WriteLine();
					Console.WriteLine($"Results were too few, let's try another prefix: '{Bright.Cyan().Underline(prefix)}'");

					foreach (IEnumerable<char> enumerable in trie.Find(prefix))
					{
						Console.WriteLine($"{++results}: " + string.Concat(enumerable));
					}
				}

				Console.WriteLine();
				Console.WriteLine($"Test remove '{Bright.Red().Underline(token)}'");

				if (!trie.Remove(token))
				{
					Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
					return;
				}

				values.Remove(token);
				results = 0;
				Console.WriteLine();
				Console.WriteLine($"Cool {Bright.Green("removed")}, let's try to find the last prefix again: '{Bright.Cyan().Underline(prefix)}'");

				foreach (IEnumerable<char> enumerable in trie.Find(prefix))
				{
					Console.WriteLine($"{++results}: " + string.Concat(enumerable));
				}

				Console.WriteLine();
				Console.WriteLine("Isn't that cool? :))");
			}
		}

		private static void TestTrieSimilarWordsRemoval()
		{
			string[] values = {
				"Car",
				"Care",
				"calcification",
				"campylobacter",
				"cartilaginous",
				"catecholamine",
				"carpetbagging",
				"carbonization",
				"catastrophism",
				"carboxylation",
				"cardiovascular",
				"cardiomyopathy",
				"capitalization",
				"carcinomatosis",
				"cardiothoracic",
				"carcinosarcoma",
				"cartelizations",
				"caprifications",
				"candlesnuffers",
				"canthaxanthins",
				"Can",
				"Canvas"
			};
			Trie<char> trie = new Trie<char>(CharComparer.InvariantCultureIgnoreCase);

			Console.Clear();
			Console.WriteLine("Adding similar words...");
			Console.WriteLine(Bright.Black("Words list: ") + string.Join(", ", values));

			foreach (string value in values) 
				trie.Add(value);

			int results = 0;
			string prefix = "car";
			Console.WriteLine();
			Console.WriteLine($"Test find '{Bright.Cyan().Underline(prefix)}'...");

			foreach (IEnumerable<char> enumerable in trie.Find(prefix))
			{
				Console.WriteLine($"{++results}: " + string.Concat(enumerable));
			}

			if (results == 0)
			{
				Console.WriteLine(Bright.Red("Didn't find a shit...!"));
				return;
			}

			string word = values[0];
			Console.WriteLine();
			Console.WriteLine($"Test remove '{Bright.Red().Underline(word)}'");

			if (!trie.Remove(word))
			{
				Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
				return;
			}

			results = 0;
			Console.WriteLine($"Cool {Bright.Green("removed")}.");
			Console.WriteLine();
			Console.WriteLine($"let's try to find the last prefix again: '{Bright.Cyan().Underline(prefix)}'");

			foreach (IEnumerable<char> enumerable in trie.Find(prefix))
			{
				Console.WriteLine($"{++results}: " + string.Concat(enumerable));
			}

			word = values[values.Length - 1];
			Console.WriteLine();
			Console.WriteLine($"Test remove '{Bright.Red().Underline(word)}'");

			if (!trie.Remove(word))
			{
				Console.WriteLine(Bright.Red("Didn't remove a shit...!"));
				return;
			}

			prefix = "ca";
			results = 0;
			Console.WriteLine($"Cool {Bright.Green("removed")}.");
			Console.WriteLine();
			Console.WriteLine($"Test find '{Bright.Cyan().Underline(prefix)}'...");

			foreach (IEnumerable<char> enumerable in trie.Find(prefix))
			{
				Console.WriteLine($"{++results}: " + string.Concat(enumerable));
			}
		}

		private static void TestSkipList()
		{
			bool more;
			Stopwatch clock = new Stopwatch();
			SkipList<int> skipList = new SkipList<int>();
			int[] values = GetRandomIntegers(true, 200_000);

			do
			{
				Console.Clear();
				Title("Testing SkipList...");
				Console.WriteLine($"Array has {values.Length} items.");
				skipList.Clear();

				int count = skipList.Count;
				Debug.Assert(count == 0, "Values are not cleared correctly!");
				clock.Restart();

				foreach (int v in values)
				{
					skipList.Add(v);
					count++;
					Debug.Assert(count == skipList.Count, $"Values are not added correctly! {count} != {skipList.Count}.");
				}

				Console.WriteLine();
				Console.WriteLine($"Added {count} items of {values.Length} in {clock.ElapsedMilliseconds} ms. Count = {skipList.Count}, Level = {skipList.Level}.");
				//skipList.WriteTo(Console.Out);

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (skipList.Contains(v))
					{
						found++;
						continue;
					}

					Console.WriteLine(Bright.Red($"Find missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}
				Console.WriteLine($"Found {found} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine();
				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (skipList.Remove(v))
					{
						removed++;
						Debug.Assert(count - removed == skipList.Count, $"Values are not removed correctly! {count} != {skipList.Count}.");
						continue;
					}
					
					Console.WriteLine(Bright.Red($"Remove missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}
				Console.WriteLine($"Removed {removed} of {count} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine();
				Console.WriteLine("Test to clear the list...");
				skipList.Clear();

				if (skipList.Count != 0)
				{
					Console.WriteLine(Bright.Red($"Something went wrong, the count is {skipList.Count}...!"));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
			
			clock.Stop();
		}
		
		private static void TestDisjointSet()
		{
			bool more;
			Stopwatch clock = new Stopwatch();
			DisjointSet<int> disjointSet = new DisjointSet<int>();
			int[] values = GetRandomIntegers(true, 12/*200_000*/);

			do
			{
				Console.Clear();
				Title("Testing DisjointSet...");
				Console.WriteLine($"Array has {values.Length} items.");
				disjointSet.Clear();

				clock.Restart();

				foreach (int v in values)
					disjointSet.Add(v);

				Console.WriteLine($"Added {disjointSet.Count} items of {values.Length} in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Yellow("Test search..."));
				int found = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (disjointSet.Contains(v))
					{
						found++;
						continue;
					}

					Console.WriteLine(Bright.Red($"Find missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}
				Console.WriteLine($"Found {found} of {disjointSet.Count} items in {clock.ElapsedMilliseconds} ms.");

				
				Console.WriteLine(Bright.Yellow("Test find and union..."));

				int threshold = (int)Math.Floor(disjointSet.Count / 0.5d);

				for (int i = 0; i < threshold; i++)
				{
					int x = IListExtension.PickRandom(values), y = IListExtension.PickRandom(values);
					Console.WriteLine($"Find {x} and {y} subsets gives {disjointSet.Find(x)} and {disjointSet.Find(y)} respectively.");
					bool connected = disjointSet.IsConnected(x, y);
					Console.WriteLine($"Are they connected? {connected.ToYesNo()}");

					if (!connected)
					{
						Console.WriteLine("Will union them.");
						disjointSet.Union(x, y);
						Console.WriteLine($"Now, are they connected? {disjointSet.IsConnected(x, y).ToYesNo()}");
					}

					Console.WriteLine();
				}

				Console.WriteLine();
				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				clock.Restart();

				foreach (int v in values)
				{
					if (disjointSet.Remove(v))
					{
						removed++;
						continue;
					}
					
					Console.WriteLine(Bright.Red($"Remove missed a value: {v} :(("));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}
				Console.WriteLine($"Removed {removed} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine();
				Console.WriteLine("Test to clear the list...");
				disjointSet.Clear();

				if (disjointSet.Count != 0)
				{
					Console.WriteLine(Bright.Red($"Something went wrong, the count is {disjointSet.Count}...!"));
					ConsoleHelper.Pause();
					Console.WriteLine();
					//return;
				}

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);
			
			clock.Stop();
		}

		private static void TestBinaryHeapAdd()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinaryHeap.Add()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				BinaryHeap<int> heap = new MaxBinaryHeap<int>();
				DoTheTest(heap, values);

				heap = new MinBinaryHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				BinaryHeap<double, Student> studentsHeap = new MaxBinaryHeap<double, Student>(e => e.Grade);
				DoTheTest(studentsHeap, students);

				studentsHeap = new MinBinaryHeap<double, Student>(e => e.Grade);
				DoTheTest(studentsHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(BinaryHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : KeyedBinaryNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue value in array)
				{
					heap.Add(value);
					//heap.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestBinaryHeapRemove()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinaryHeap.Remove()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				BinaryHeap<int> heap = new MaxBinaryHeap<int>();
				DoTheTest(heap, values);

				heap = new MinBinaryHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				BinaryHeap<double, Student> studentsHeap = new MaxBinaryHeap<double, Student>(e => e.Grade);
				DoTheTest(studentsHeap, students);

				studentsHeap = new MinBinaryHeap<double, Student>(e => e.Grade);
				DoTheTest(studentsHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(BinaryHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : KeyedBinaryNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine("Test removing...");
				bool removeStarted = false;

				while (heap.Count > 0)
				{
					if (!removeStarted) removeStarted = true;
					else Console.Write(", ");

					Console.Write(heap.ExtractValue());
				}

				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestBinaryHeapElementAt()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinaryHeap ElementAt...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				int k = RNGRandomHelper.Next(1, values.Length);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				BinaryHeap<int> heap = new MaxBinaryHeap<int>();
				DoTheTest(heap, values, k);

				heap = new MinBinaryHeap<int>();
				DoTheTest(heap, values, k);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				BinaryHeap<double, Student> studentHeap = new MaxBinaryHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				studentHeap = new MinBinaryHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(BinaryHeap<TNode, TKey, TValue> heap, TValue[] array, int k)
				where TNode : KeyedBinaryNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine($"Kth element at position {k} = {Bright.Cyan().Underline(heap.ElementAt(k).ToString())}");
				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestBinaryHeapDecreaseKey()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinaryHeap DecreaseKey...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				BinaryHeap<int> heap = new MaxBinaryHeap<int>();
				DoTheValueTest(heap, values, int.MaxValue);

				heap = new MinBinaryHeap<int>();
				DoTheValueTest(heap, values, int.MinValue);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				BinaryHeap<double, Student> studentHeap = new MaxBinaryHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MaxValue);

				studentHeap = new MinBinaryHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MinValue);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheKeyTest<TNode, TKey, TValue>(BinaryHeap<TNode, TKey, TValue> heap, TValue[] array, TKey newKeyValue)
				where TNode : KeyedBinaryNode<TNode, TKey, TValue>
			{
				Queue<TKey> queue = new Queue<TKey>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TKey key = queue.Dequeue();
					TNode node = heap.FindByKey(key);
					Debug.Assert(node != null, $"Node for key {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TKey extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, key);
					Console.WriteLine($"Extracted {extracted}, expected {key}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {key}.");
				}

				Console.WriteLine();
			}

			static void DoTheValueTest<TNode, TValue>(BinaryHeap<TNode, TValue, TValue> heap, TValue[] array, TValue newKeyValue)
				where TNode : KeyedBinaryNode<TNode, TValue, TValue>
			{
				Queue<TValue> queue = new Queue<TValue>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TValue key = queue.Dequeue();
					TNode node = heap.Find(key);
					Debug.Assert(node != null, $"Node for value {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TValue extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, newKeyValue);
					Console.WriteLine($"Extracted {extracted}, expected {newKeyValue}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {node.Value}.");
				}

				Console.WriteLine();
			}

			static void DoTheTest<TNode, TKey, TValue>(BinaryHeap<TNode, TKey, TValue> heap, TValue[] array, Queue<TKey> queue)
				where TNode : KeyedBinaryNode<TNode, TKey, TValue>
			{
				const int MAX = 10;

				int max = Math.Min(MAX, array.Length);
				queue.Clear();
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue v in array)
				{
					TNode node = heap.MakeNode(v);
					if (queue.Count < max) queue.Enqueue(node.Key);
					heap.Add(node);
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestBinomialHeapAdd()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinomialHeap.Add()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				BinomialHeap<int> heap = new MaxBinomialHeap<int>();
				DoTheTest(heap, values);

				heap = new MinBinomialHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				BinomialHeap<double, Student> studentHeap = new MaxBinomialHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				studentHeap = new MinBinomialHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(BinomialHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : BinomialNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue value in array)
				{
					heap.Add(value);
					//heap.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestBinomialHeapRemove()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinomialHeap.Remove()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				BinomialHeap<int> heap = new MaxBinomialHeap<int>();
				DoTheTest(heap, values);

				heap = new MinBinomialHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));

				BinomialHeap<double, Student> studentHeap = new MaxBinomialHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				studentHeap = new MinBinomialHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(BinomialHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : BinomialNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine("Test removing...");
				bool removeStarted = false;

				while (heap.Count > 0)
				{
					if (!removeStarted) removeStarted = true;
					else Console.Write(", ");

					Console.Write(heap.ExtractValue());
				}

				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestBinomialHeapElementAt()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinomialHeap ElementAt...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				int k = RNGRandomHelper.Next(1, values.Length);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				BinomialHeap<int> heap = new MaxBinomialHeap<int>();
				DoTheTest(heap, values, k);

				heap = new MinBinomialHeap<int>();
				DoTheTest(heap, values, k);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				BinomialHeap<double, Student> studentHeap = new MaxBinomialHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				studentHeap = new MinBinomialHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(BinomialHeap<TNode, TKey, TValue> heap, TValue[] array, int k)
				where TNode : BinomialNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine($"Kth element at position {k} element = {Bright.Cyan().Underline(heap.ElementAt(k).ToString())}");
				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestBinomialHeapDecreaseKey()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing BinomialHeap DecreaseKey...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				BinomialHeap<int> heap = new MaxBinomialHeap<int>();
				DoTheValueTest(heap, values, int.MaxValue);

				heap = new MinBinomialHeap<int>();
				DoTheValueTest(heap, values, int.MinValue);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				BinomialHeap<double, Student> studentHeap = new MaxBinomialHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MaxValue);

				studentHeap = new MinBinomialHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MinValue);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheKeyTest<TNode, TKey, TValue>(BinomialHeap<TNode, TKey, TValue> heap, TValue[] array, TKey newKeyValue)
				where TNode : BinomialNode<TNode, TKey, TValue>
			{
				Queue<TKey> queue = new Queue<TKey>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TKey key = queue.Dequeue();
					TNode node = heap.FindByKey(key);
					Debug.Assert(node != null, $"Node for key {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TKey extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, key);
					Console.WriteLine($"Extracted {extracted}, expected {key}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {key}.");
				}

				Console.WriteLine();
			}

			static void DoTheValueTest<TNode, TValue>(BinomialHeap<TNode, TValue, TValue> heap, TValue[] array, TValue newKeyValue)
				where TNode : BinomialNode<TNode, TValue, TValue>
			{
				Queue<TValue> queue = new Queue<TValue>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TValue key = queue.Dequeue();
					TNode node = heap.Find(key);
					Debug.Assert(node != null, $"Node for value {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TValue extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, newKeyValue);
					Console.WriteLine($"Extracted {extracted}, expected {newKeyValue}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {node.Value}.");
				}

				Console.WriteLine();
			}

			static void DoTheTest<TNode, TKey, TValue>(BinomialHeap<TNode, TKey, TValue> heap, TValue[] array, Queue<TKey> queue)
				where TNode : BinomialNode<TNode, TKey, TValue>
			{
				const int MAX = 10;

				int max = Math.Min(MAX, array.Length);
				queue.Clear();
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue v in array)
				{
					TNode node = heap.MakeNode(v);
					if (queue.Count < max) queue.Enqueue(node.Key);
					heap.Add(node);
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestPairingHeapAdd()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing PairingHeap.Add()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				PairingHeap<int> heap = new MaxPairingHeap<int>();
				DoTheTest(heap, values);

				heap = new MinPairingHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				PairingHeap<double, Student> studentHeap = new MaxPairingHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				studentHeap = new MinPairingHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(PairingHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : PairingNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue value in array)
				{
					heap.Add(value);
					//heap.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestPairingHeapRemove()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing PairingHeap.Remove()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				PairingHeap<int> heap = new MaxPairingHeap<int>();
				DoTheTest(heap, values);

				heap = new MinPairingHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));

				PairingHeap<double, Student> studentHeap = new MaxPairingHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				studentHeap = new MinPairingHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(PairingHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : PairingNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine("Test removing...");
				bool removeStarted = false;

				while (heap.Count > 0)
				{
					if (!removeStarted) removeStarted = true;
					else Console.Write(", ");

					Console.Write(heap.ExtractValue());
				}

				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestPairingHeapElementAt()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing PairingHeap ElementAt...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				int k = RNGRandomHelper.Next(1, values.Length);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				PairingHeap<int> heap = new MaxPairingHeap<int>();
				DoTheTest(heap, values, k);

				heap = new MinPairingHeap<int>();
				DoTheTest(heap, values, k);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				PairingHeap<double, Student> studentHeap = new MaxPairingHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				studentHeap = new MinPairingHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(PairingHeap<TNode, TKey, TValue> heap, TValue[] array, int k)
				where TNode : PairingNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine($"Kth element at position {k} element = {Bright.Cyan().Underline(heap.ElementAt(k).ToString())}");
				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestPairingHeapDecreaseKey()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing PairingHeap DecreaseKey...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				PairingHeap<int> heap = new MaxPairingHeap<int>();
				DoTheValueTest(heap, values, int.MaxValue);

				heap = new MinPairingHeap<int>();
				DoTheValueTest(heap, values, int.MinValue);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				PairingHeap<double, Student> studentHeap = new MaxPairingHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MaxValue);

				studentHeap = new MinPairingHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MinValue);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheKeyTest<TNode, TKey, TValue>(PairingHeap<TNode, TKey, TValue> heap, TValue[] array, TKey newKeyValue)
				where TNode : PairingNode<TNode, TKey, TValue>
			{
				Queue<TKey> queue = new Queue<TKey>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TKey key = queue.Dequeue();
					TNode node = heap.FindByKey(key);
					Debug.Assert(node != null, $"Node for key {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TKey extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, key);
					Console.WriteLine($"Extracted {extracted}, expected {key}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {key}.");
				}

				Console.WriteLine();
			}

			static void DoTheValueTest<TNode, TValue>(PairingHeap<TNode, TValue, TValue> heap, TValue[] array, TValue newKeyValue)
				where TNode : PairingNode<TNode, TValue, TValue>
			{
				Queue<TValue> queue = new Queue<TValue>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TValue key = queue.Dequeue();
					TNode node = heap.Find(key);
					Debug.Assert(node != null, $"Node for value {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TValue extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, newKeyValue);
					Console.WriteLine($"Extracted {extracted}, expected {newKeyValue}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {node.Value}.");
				}

				Console.WriteLine();
			}

			static void DoTheTest<TNode, TKey, TValue>(PairingHeap<TNode, TKey, TValue> heap, TValue[] array, Queue<TKey> queue)
				where TNode : PairingNode<TNode, TKey, TValue>
			{
				const int MAX = 10;

				int max = Math.Min(MAX, array.Length);
				queue.Clear();
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue v in array)
				{
					TNode node = heap.MakeNode(v);
					if (queue.Count < max) queue.Enqueue(node.Key);
					heap.Add(node);
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestFibonacciHeapAdd()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing FibonacciHeap.Add()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				FibonacciHeap<int> heap = new MaxFibonacciHeap<int>();
				DoTheTest(heap, values);

				heap = new MinFibonacciHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				FibonacciHeap<double, Student> studentHeap = new MaxFibonacciHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				studentHeap = new MinFibonacciHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(FibonacciHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : FibonacciNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue value in array)
				{
					heap.Add(value);
					//heap.PrintWithProps();
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		private static void TestFibonacciHeapRemove()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing FibonacciHeap.Remove()...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

				FibonacciHeap<int> heap = new MaxFibonacciHeap<int>();
				DoTheTest(heap, values);

				heap = new MinFibonacciHeap<int>();
				DoTheTest(heap, values);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));

				FibonacciHeap<double, Student> studentHeap = new MaxFibonacciHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				studentHeap = new MinFibonacciHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(FibonacciHeap<TNode, TKey, TValue> heap, TValue[] array)
				where TNode : FibonacciNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine("Test removing...");
				bool removeStarted = false;

				while (heap.Count > 0)
				{
					if (!removeStarted) removeStarted = true;
					else Console.Write(", ");

					Console.Write(heap.ExtractValue());
				}

				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestFibonacciHeapElementAt()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing FibonacciHeap ElementAt...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				int k = RNGRandomHelper.Next(1, values.Length);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				FibonacciHeap<int> heap = new MaxFibonacciHeap<int>();
				DoTheTest(heap, values, k);

				heap = new MinFibonacciHeap<int>();
				DoTheTest(heap, values, k);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				FibonacciHeap<double, Student> studentHeap = new MaxFibonacciHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				studentHeap = new MinFibonacciHeap<double, Student>(e => e.Grade);
				DoTheTest(studentHeap, students, k);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheTest<TNode, TKey, TValue>(FibonacciHeap<TNode, TKey, TValue> heap, TValue[] array, int k)
				where TNode : FibonacciNode<TNode, TKey, TValue>
			{
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));
				heap.Add(array);
				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
				Console.WriteLine($"Kth element at position {k} element = {Bright.Cyan().Underline(heap.ElementAt(k).ToString())}");
				Console.WriteLine();
				Console.WriteLine();
			}
		}

		private static void TestFibonacciHeapDecreaseKey()
		{
			bool more;

			do
			{
				Console.Clear();
				Title("Testing FibonacciHeap DecreaseKey...");

				int len = RNGRandomHelper.Next(1, 12);
				int[] values = GetRandomIntegers(len);
				Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
				Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

				FibonacciHeap<int> heap = new MaxFibonacciHeap<int>();
				DoTheValueTest(heap, values, int.MaxValue);

				heap = new MinFibonacciHeap<int>();
				DoTheValueTest(heap, values, int.MinValue);

				Student[] students = GetRandomStudents(len);
				Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
				Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

				FibonacciHeap<double, Student> studentHeap = new MaxFibonacciHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MaxValue);

				studentHeap = new MinFibonacciHeap<double, Student>(e => e.Grade);
				DoTheKeyTest(studentHeap, students, int.MinValue);

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
			}
			while (more);

			static void DoTheKeyTest<TNode, TKey, TValue>(FibonacciHeap<TNode, TKey, TValue> heap, TValue[] array, TKey newKeyValue)
				where TNode : FibonacciNode<TNode, TKey, TValue>
			{
				Queue<TKey> queue = new Queue<TKey>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TKey key = queue.Dequeue();
					TNode node = heap.FindByKey(key);
					Debug.Assert(node != null, $"Node for key {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TKey extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, key);
					Console.WriteLine($"Extracted {extracted}, expected {key}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {key}.");
				}

				Console.WriteLine();
			}

			static void DoTheValueTest<TNode, TValue>(FibonacciHeap<TNode, TValue, TValue> heap, TValue[] array, TValue newKeyValue)
				where TNode : FibonacciNode<TNode, TValue, TValue>
			{
				Queue<TValue> queue = new Queue<TValue>();
				DoTheTest(heap, array, queue);

				while (queue.Count > 0)
				{
					TValue key = queue.Dequeue();
					TNode node = heap.Find(key);
					Debug.Assert(node != null, $"Node for value {key} is not found.");
					heap.DecreaseKey(node, newKeyValue);
					TValue extracted = heap.ExtractValue().Key;
					bool succeeded = heap.Comparer.IsEqual(extracted, newKeyValue);
					Console.WriteLine($"Extracted {extracted}, expected {newKeyValue}");
					Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {node.Value}.");
				}

				Console.WriteLine();
			}

			static void DoTheTest<TNode, TKey, TValue>(FibonacciHeap<TNode, TKey, TValue> heap, TValue[] array, Queue<TKey> queue)
				where TNode : FibonacciNode<TNode, TKey, TValue>
			{
				const int MAX = 10;

				int max = Math.Min(MAX, array.Length);
				queue.Clear();
				Console.WriteLine(Bright.Green($"Test adding ({heap.GetType().Name})..."));

				foreach (TValue v in array)
				{
					TNode node = heap.MakeNode(v);
					if (queue.Count < max) queue.Enqueue(node.Key);
					heap.Add(node);
				}

				Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
				heap.Print();
			}
		}

		#region Not working
		/*
		 * something is off about this class!
		 * I'm 100% sure there must be a bug in there because it can't be right to refer
		 * to _pq[1] instead of _pq[0] while it uses freely the zero based offset!
		 * I'm not sure if the original code works but the idea is cool. It might perform
		 * better but it'll take time to adjust it. Maybe later.
		 */
		//private static void TestIndexMinAdd()
		//{
		//	bool more;

		//	do
		//	{
		//		Console.Clear();
		//		Title("Testing IndexMin.Add()...");

		//		int len = RNGRandomHelper.Next(1, 12);
		//		int[] values = GetRandomIntegers(len);
		//		Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

		//		IndexMin<int> heap = new MaxIndexMin<int>();
		//		DoTheTest(heap, values);

		//		heap = new MinIndexMin<int>();
		//		DoTheTest(heap, values);

		//		Student[] students = GetRandomStudents(len);
		//		IndexMin<double, Student> studentsHeap = new MaxIndexMin<double, Student>(e => e.Grade);
		//		DoTheTest(studentsHeap, students);

		//		studentsHeap = new MinIndexMin<double, Student>(e => e.Grade);
		//		DoTheTest(studentsHeap, students);

		//		Console.WriteLine();
		//		Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
		//		ConsoleKeyInfo response = Console.ReadKey(true);
		//		Console.WriteLine();
		//		more = response.Key == ConsoleKey.Y;
		//	}
		//	while (more);

		//	static void DoTheTest<TNode, TKey, TValue>(IndexMin<TNode, TKey, TValue> heap, TValue[] array)
		//		where TNode : KeyedBinaryNode<TNode, TKey, TValue>
		//	{
		//		Console.WriteLine($"Test adding ({heap.GetType().Name})...".Bright.Green());

		//		foreach (TValue value in array)
		//		{
		//			heap.Add(value);
		//			//heap.PrintWithProps();
		//		}

		//		Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
		//	}
		//}

		//private static void TestIndexMinRemove()
		//{
		//	bool more;

		//	do
		//	{
		//		Console.Clear();
		//		Title("Testing IndexMin.Remove()...");

		//		int len = RNGRandomHelper.Next(1, 12);
		//		int[] values = GetRandomIntegers(len);
		//		Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));

		//		IndexMin<int> heap = new MaxIndexMin<int>();
		//		DoTheTest(heap, values);

		//		heap = new MinIndexMin<int>();
		//		DoTheTest(heap, values);

		//		Student[] students = GetRandomStudents(len);
		//		IndexMin<double, Student> studentsHeap = new MaxIndexMin<double, Student>(e => e.Grade);
		//		DoTheTest(studentsHeap, students);

		//		studentsHeap = new MinIndexMin<double, Student>(e => e.Grade);
		//		DoTheTest(studentsHeap, students);

		//		Console.WriteLine();
		//		Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
		//		ConsoleKeyInfo response = Console.ReadKey(true);
		//		Console.WriteLine();
		//		more = response.Key == ConsoleKey.Y;
		//	}
		//	while (more);

		//	static void DoTheTest<TNode, TKey, TValue>(IndexMin<TNode, TKey, TValue> heap, TValue[] array)
		//		where TNode : KeyedBinaryNode<TNode, TKey, TValue>
		//	{
		//		Console.WriteLine($"Test adding ({heap.GetType().Name})...".Bright.Green());
		//		heap.Add(array);
		//		Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
		//		Console.WriteLine("Test removing...");
		//		bool removeStarted = false;

		//		while (heap.Count > 0)
		//		{
		//			if (!removeStarted) removeStarted = true;
		//			else Console.Write(", ");

		//			Console.Write(heap.ExtractValue());
		//		}

		//		Console.WriteLine();
		//		Console.WriteLine();
		//	}
		//}

		//private static void TestIndexMinElementAt()
		//{
		//	bool more;

		//	do
		//	{
		//		Console.Clear();
		//		Title("Testing IndexMin ElementAt...");

		//		int len = RNGRandomHelper.Next(1, 12);
		//		int[] values = GetRandomIntegers(len);
		//		int k = RNGRandomHelper.Next(1, values.Length);
		//		Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
		//		Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

		//		IndexMin<int> heap = new MaxIndexMin<int>();
		//		DoTheTest(heap, values, k);

		//		heap = new MinIndexMin<int>();
		//		DoTheTest(heap, values, k);

		//		Student[] students = GetRandomStudents(len);
		//		Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
		//		Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

		//		IndexMin<double, Student> studentHeap = new MaxIndexMin<double, Student>(e => e.Grade);
		//		DoTheTest(studentHeap, students, k);

		//		studentHeap = new MinIndexMin<double, Student>(e => e.Grade);
		//		DoTheTest(studentHeap, students, k);

		//		Console.WriteLine();
		//		Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
		//		ConsoleKeyInfo response = Console.ReadKey(true);
		//		Console.WriteLine();
		//		more = response.Key == ConsoleKey.Y;
		//	}
		//	while (more);

		//	static void DoTheTest<TNode, TKey, TValue>(IndexMin<TNode, TKey, TValue> heap, TValue[] array, int k)
		//		where TNode : KeyedBinaryNode<TNode, TKey, TValue>
		//	{
		//		Console.WriteLine($"Test adding ({heap.GetType().Name})...".Bright.Green());
		//		heap.Add(array);
		//		Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
		//		Console.WriteLine($"Kth element at position {k} = {heap.ElementAt(k).ToString().Bright.Cyan().Underline()}");
		//		Console.WriteLine();
		//		Console.WriteLine();
		//	}
		//}

		//private static void TestIndexMinDecreaseKey()
		//{
		//	bool more;

		//	do
		//	{
		//		Console.Clear();
		//		Title("Testing IndexMin DecreaseKey...");

		//		int len = RNGRandomHelper.Next(1, 12);
		//		int[] values = GetRandomIntegers(len);
		//		Console.WriteLine(Bright.Black("Array: ") + string.Join(", ", values));
		//		Console.WriteLine(Yellow("Array [sorted]: ") + string.Join(", ", values.OrderBy(e => e)));

		//		IndexMin<int> heap = new MaxIndexMin<int>();
		//		DoTheValueTest(heap, values, int.MaxValue);

		//		heap = new MinIndexMin<int>();
		//		DoTheValueTest(heap, values, int.MinValue);

		//		Student[] students = GetRandomStudents(len);
		//		Console.WriteLine(Bright.Black("Students: ") + string.Join(", ", students.Select(e => $"{e.Name} {e.Grade:F2}")));
		//		Console.WriteLine(Yellow("Students [sorted]: ") + string.Join(", ", students.OrderBy(e => e.Grade).Select(e => $"{e.Name} {e.Grade:F2}")));

		//		IndexMin<double, Student> studentHeap = new MaxIndexMin<double, Student>(e => e.Grade);
		//		DoTheKeyTest(studentHeap, students, int.MaxValue, e => e.Grade);

		//		studentHeap = new MinIndexMin<double, Student>(e => e.Grade);
		//		DoTheKeyTest(studentHeap, students, int.MinValue, e => e.Grade);

		//		Console.WriteLine();
		//		Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
		//		ConsoleKeyInfo response = Console.ReadKey(true);
		//		Console.WriteLine();
		//		more = response.Key == ConsoleKey.Y;
		//	}
		//	while (more);

		//	static void DoTheKeyTest<TNode, TKey, TValue>(IndexMin<TNode, TKey, TValue> heap, TValue[] array, TKey newKeyValue, Func<TValue, TKey> getKey)
		//		where TNode : KeyedBinaryNode<TNode, TKey, TValue>
		//	{
		//		Queue<TKey> queue = new Queue<TKey>();
		//		DoTheTest(heap, array, queue);

		//		bool succeeded = true;

		//		while (succeeded && queue.Count > 0)
		//		{
		//			TKey key = queue.Dequeue();
		//			TNode node = heap.FindByKey(key);
		//			Debug.Assert(node != null, $"Node for key {key} is not found.");
		//			heap.DecreaseKey(node, newKeyValue);
		//			TKey extracted = heap.ExtractValue().Key;
		//			succeeded = heap.Comparer.IsEqual(extracted, key);
		//			Console.WriteLine($"Extracted {extracted}, expected {key}");
		//			Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {key}.");
		//		}

		//		Console.WriteLine();
		//	}

		//	static void DoTheValueTest<TNode, TValue>(IndexMin<TNode, TValue, TValue> heap, TValue[] array, TValue newKeyValue)
		//		where TNode : KeyedBinaryNode<TNode, TValue, TValue>
		//	{
		//		Queue<TValue> queue = new Queue<TValue>();
		//		DoTheTest(heap, array, queue);

		//		bool succeeded = true;

		//		while (succeeded && queue.Count > 0)
		//		{
		//			TValue key = queue.Dequeue();
		//			TNode node = heap.Find(key);
		//			Debug.Assert(node != null, $"Node for value {key} is not found.");
		//			heap.DecreaseKey(node, newKeyValue);
		//			TValue extracted = heap.ExtractValue().Key;
		//			succeeded = heap.Comparer.IsEqual(extracted, newKeyValue);
		//			Console.WriteLine($"Extracted {extracted}, expected {newKeyValue}");
		//			Debug.Assert(succeeded, $"Extracted a different value {extracted} instead of {node.Value}.");
		//		}

		//		Console.WriteLine();
		//	}

		//	static void DoTheTest<TNode, TKey, TValue>(IndexMin<TNode, TKey, TValue> heap, TValue[] array, Queue<TKey> queue)
		//		where TNode : KeyedBinaryNode<TNode, TKey, TValue>
		//	{
		//		const int MAX = 10;

		//		int max = Math.Min(MAX, array.Length);
		//		queue.Clear();
		//		Console.WriteLine($"Test adding ({heap.GetType().Name})...".Bright.Green());

		//		foreach (TValue v in array)
		//		{
		//			TNode node = heap.MakeNode(v);
		//			if (queue.Count < max) queue.Enqueue(node.Key);
		//			heap.Add(node);
		//		}

		//		Console.WriteLine(Bright.Black("Enumeration: ") + string.Join(", ", heap));
		//	}
		//}
		#endregion

		private static void TestAllHeapsPerformance()
		{
			bool more;
			Stopwatch clock = new Stopwatch();
			int tests = 0;
			int[] values = GetRandomIntegers(true, START);
			IDictionary<string, long> result = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
			Student[] students = GetRandomStudents(START);
			Func<Student, double> getKey = e => e.Grade;

			do
			{
				Console.Clear();
				Title("Testing All Heap types performance...");
				if (tests == 0) CompilationHint();
				Console.WriteLine($"Array has {values.Length} items.");
				Title("Testing IHeap<int> types performance...");
				result.Clear();

				// BinaryHeap
				DoHeapTest(new MinBinaryHeap<int>(), values, clock);
				result[typeof(MinBinaryHeap<int>).Name] = clock.ElapsedTicks;
				clock.Stop();

				// BinomialHeap
				DoHeapTest(new MinBinomialHeap<int>(), values, clock);
				result[typeof(MinBinomialHeap<int>).Name] = clock.ElapsedTicks;
				clock.Stop();

				// PairingHeap
				DoHeapTest(new MinPairingHeap<int>(), values, clock);
				result[typeof(MinPairingHeap<int>).Name] = clock.ElapsedTicks;
				clock.Stop();

				// FibonacciHeap
				DoHeapTest(new MinFibonacciHeap<int>(), values, clock);
				result[typeof(MinFibonacciHeap<int>).Name] = clock.ElapsedTicks;
				clock.Stop();

				Console.WriteLine();
				Console.WriteLine("Results for Heap<int>:");

				foreach (KeyValuePair<string, long> pair in result.OrderBy(e => e.Value))
				{
					Console.WriteLine($"{pair.Key} took {pair.Value} ticks");
				}

				ConsoleHelper.Pause();

				Title("Testing IKeyedHeap<TNode, TKey, TValue> types performance...");
				result.Clear();
				
				// BinaryHeap
				DoHeapTest(new MinBinaryHeap<double, Student>(getKey), students, clock);
				result[typeof(MinBinaryHeap<double, Student>).Name] = clock.ElapsedTicks;
				clock.Stop();

				// BinomialHeap
				DoHeapTest(new MinBinomialHeap<double, Student>(getKey), students, clock);
				result[typeof(MinBinomialHeap<double, Student>).Name] = clock.ElapsedTicks;
				clock.Stop();

				// PairingHeap
				DoHeapTest(new MinPairingHeap<double, Student>(getKey), students, clock);
				result[typeof(MinPairingHeap<double, Student>).Name] = clock.ElapsedTicks;
				clock.Stop();

				// FibonacciHeap
				DoHeapTest(new MinFibonacciHeap<double, Student>(getKey), students, clock);
				result[typeof(MinFibonacciHeap<double, Student>).Name] = clock.ElapsedTicks;
				clock.Stop();

				Console.WriteLine();
				Console.WriteLine("Results for Heap<double, Student>:");

				foreach (KeyValuePair<string, long> pair in result.OrderBy(e => e.Value))
				{
					Console.WriteLine($"{pair.Key} took {pair.Value} ticks");
				}

				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || tests > 0) continue;
				values = GetRandomIntegers(true, HEAVY);
				students = GetRandomStudents(HEAVY);
				tests++;
			}
			while (more);

			clock.Stop();

			static void DoHeapTest<T>(IHeap<T> heap, T[] values, Stopwatch clock)
			{
				Console.WriteLine();
				Console.WriteLine(Bright.Green($"Testing {heap.GetType().Name}..."));
				heap.Clear();
				Console.WriteLine($"Original values: {Bright.Yellow(values.Length.ToString())}...");
				Debug.Assert(heap.Count == 0, "Values are not cleared correctly!");

				clock.Restart();
				heap.Add(values);
				Console.WriteLine($"Added {heap.Count} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");

				Console.WriteLine(Bright.Red("Test removing..."));
				int removed = 0;
				clock.Restart();

				while (heap.Count > 0)
				{
					heap.ExtractValue();
					removed++;
				}
				Console.WriteLine($"Removed {removed} of {values.Length} items in {clock.ElapsedMilliseconds} ms.");
				Debug.Assert(removed == values.Length, "Not all values are removed correctly!");
			}
		}

		private static void TestGraph()
		{
			const int MAX_LIST = 26;

			bool more;
			GraphList<char> graph;
			WeightedGraphList<char, int> weightedGraph;
			List<char> values = new List<char>();
			Menu menu = new Menu()
				.Add("Undirected graph", () =>
				{
					Console.WriteLine();
					graph = new UndirectedGraphList<char>();
					if (values.Count == 0) AddValues(values);
					DoTheTest(graph, values);
				})
				.Add("Directed graph", () =>
				{
					Console.WriteLine();
					graph = new DirectedGraphList<char>();
					if (values.Count == 0) AddValues(values);
					DoTheTest(graph, values);
				})
				.Add("Mixed graph", () =>
				{
					Console.WriteLine();
					graph = new MixedGraphList<char>();
					if (values.Count == 0) AddValues(values);
					DoTheTest(graph, values);
				})
				.Add("Weighted undirected graph", () =>
				{
					Console.WriteLine();
					weightedGraph = new WeightedUndirectedGraphList<char, int>();
					if (values.Count == 0) AddValues(values);
					DoTheTest(weightedGraph, values);
				})
				.Add("Weighted directed graph", () =>
				{
					Console.WriteLine();
					weightedGraph = new WeightedDirectedGraphList<char, int>();
					if (values.Count == 0) AddValues(values);
					DoTheTest(weightedGraph, values);
				})
				.Add("Weighted mixed graph", () =>
				{
					Console.WriteLine();
					weightedGraph = new WeightedMixedGraphList<char, int>();
					if (values.Count == 0) AddValues(values);
					DoTheTest(weightedGraph, values);
				});

			do
			{
				Console.Clear();
				Title("Testing graph add()");
				menu.Display();
				Console.WriteLine();
				Console.Write($"Press {Bright.Green("[Y]")} to make another test or {Dim("any other key")} to exit. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				more = response.Key == ConsoleKey.Y;
				if (!more || values.Count >= MAX_LIST) continue;

				Console.Write($"Would you like to add more character? {Bright.Green("[Y]")} / {Dim("any key")} ");
				response = Console.ReadKey(true);
				Console.WriteLine();
				if (response.Key != ConsoleKey.Y) continue;
				Console.WriteLine();
				AddValues(values);
			}
			while (more);

			static void AddValues(List<char> list)
			{
				int len = RNGRandomHelper.Next(1, 12);
				Console.WriteLine(Bright.Green("Generating new characters: "));
				char[] newValues = GetRandomChar(true, len);
				int count = 0;

				foreach (char value in newValues)
				{
					if (list.Contains(value)) continue;
					list.Add(value);
					count++;
				}

				Console.WriteLine(Bright.Green($"Added {count} characters to the set"));
			}

			static void DoTheTest<TAdjacencyList, TEdge>(GraphList<char, TAdjacencyList, TEdge> graph, List<char> values)
				where TAdjacencyList : class, ICollection<TEdge>
			{
				Console.WriteLine("Test adding nodes...");
				Console.WriteLine(Bright.Black("characters list: ") + string.Join(", ", values));
				graph.Clear();
				graph.Add(values);

				if (graph.Count != values.Count)
				{
					Console.WriteLine(Bright.Red("Something went wrong, not all nodes were added...!"));
					return;
				}

				if (graph.Count == 1)
				{
					Console.WriteLine(Bright.Red("Huh, must add more nodes...!"));
					return;
				}
				
				Console.WriteLine(Bright.Green("All nodes are added...!") + " Let's try adding some relationships...");
				Console.Write($@"{Yellow("Would you like to add a bit of randomization?")} {Bright.Green("[Y]")} / {Dim("any key")}.
This may cause cycles but will make it much more fun for finding shortest paths. ");
				ConsoleKeyInfo response = Console.ReadKey(true);
				Console.WriteLine();
				int threshold = response.Key != ConsoleKey.Y ? 0 : (int)Math.Floor(values.Count * 0.5d);

				Console.Write($"{Yellow("Can the edges have negative weights?")} {Bright.Green("[Y]")} / {Dim("any key")}.");
				response = Console.ReadKey(true);
				Console.WriteLine();
				int min = response.Key == ConsoleKey.Y
							? (int)sbyte.MinValue
							: byte.MinValue, max = sbyte.MaxValue;

				Queue<char> queue = new Queue<char>(values);
				char from = queue.Dequeue();
				Action<char, char> addEdge = graph switch
				{
					MixedGraphList<char> mGraph => (f, t) => mGraph.AddEdge(f, t, __fakeGenerator.Value.Random.Bool()),
					WeightedMixedGraphList<char, int> wmGraph => (f, t) => wmGraph.AddEdge(f, t, RNGRandomHelper.Next(min, max), __fakeGenerator.Value.Random.Bool()),
					WeightedGraphList<char, int> wGraph => (f, t) => wGraph.AddEdge(f, t, RNGRandomHelper.Next(min, max)),
					_ => graph.AddEdge
				};

				while (queue.Count > 0)
				{
					char to = queue.Dequeue();
					if (graph.ContainsEdge(from, to)) continue;
					Console.WriteLine($"Adding {Bright.Cyan().Underline(from.ToString())} to {Bright.Cyan().Underline(to.ToString())}...");
					addEdge(from, to);

					if (threshold > 0 && __fakeGenerator.Value.Random.Bool())
					{
						queue.Enqueue(from);
						queue.Enqueue(IListExtension.PickRandom(values));
						threshold--;
					}

					from = to;
				}

				graph.Print();
				Console.WriteLine();
				Console.WriteLine("Cool, let's try enumerating it.");
				char value = graph.Top().First();
				Console.WriteLine($"Picking a value with maximum connections: '{Bright.Cyan().Underline(value.ToString())}'...");
				if (!DoTheTestWithValue(graph, values, value)) return;

				do
				{
					Console.WriteLine();
					Console.Write($@"Type in {Bright.Green("a character")} to traverse from,
or press {Bright.Red("ESCAPE")} key to exit this test. ");
					response = Console.ReadKey();
					Console.WriteLine();
					if (response.Key == ConsoleKey.Escape) continue;

					if (!char.IsLetter(response.KeyChar) || !graph.ContainsKey(response.KeyChar))
					{
						Console.WriteLine($"Character '{value}' is not found or not connected!");
						continue;
					}

					value = response.KeyChar;
					if (!DoTheTestWithValue(graph, values, value)) return;
				}
				while (response.Key != ConsoleKey.Escape);

				Console.WriteLine();
			}

			static bool DoTheTestWithValue<TAdjacencyList, TEdge>(GraphList<char, TAdjacencyList, TEdge> graph, List<char> values, char value)
				where TAdjacencyList : class, ICollection<TEdge>
			{
				const string LINE_SEPARATOR = "*******************************************************************************";

				Console.WriteLine(Yellow("Breadth First: ") + string.Join(", ", graph.Enumerate(value, BreadthDepthTraversal.BreadthFirst)));
				Console.WriteLine(Yellow("Depth First: ") + string.Join(", ", graph.Enumerate(value, BreadthDepthTraversal.DepthFirst)));
				Console.WriteLine(Yellow("Degree: ") + graph.Degree(value));

				// detect a cycle
				IEnumerable<char> cycle = graph.FindCycle();
				if (cycle != null) Console.WriteLine(Bright.Red("Found cycle: ") + string.Join(", ", cycle));

				// test specific graph type features
				switch (graph)
				{
					case DirectedGraphList<char> directedGraph:
						Console.WriteLine(Yellow("InDegree: ") + directedGraph.InDegree(value));
						try { Console.WriteLine(Yellow("Topological Sort: ") + string.Join(", ", directedGraph.TopologicalSort())); }
						catch (Exception e) { Console.WriteLine(Yellow("Topological Sort: ") + Bright.Red(e.Message)); }
						break;
					case MixedGraphList<char> mixedGraph:
						Console.WriteLine(Yellow("InDegree: ") + mixedGraph.InDegree(value));
						try { Console.WriteLine(Yellow("Topological Sort: ") + string.Join(", ", mixedGraph.TopologicalSort())); }
						catch (Exception e) { Console.WriteLine(Yellow("Topological Sort: ") + Bright.Red(e.Message)); }
						break;
					case WeightedDirectedGraphList<char, int> weightedDirectedGraph:
						Console.WriteLine(Yellow("InDegree: ") + weightedDirectedGraph.InDegree(value));
						try { Console.WriteLine(Yellow("Topological Sort: ") + string.Join(", ", weightedDirectedGraph.TopologicalSort())); }
						catch (Exception e) { Console.WriteLine(Yellow("Topological Sort: ") + Bright.Red(e.Message)); }
						break;
					case WeightedUndirectedGraphList<char, int> weightedUndirectedGraph:
						Console.WriteLine(LINE_SEPARATOR);
						WeightedUndirectedGraphList<char, int> spanningTree = weightedUndirectedGraph.GetMinimumSpanningTree(SpanningTreeAlgorithm.Prim);

						if (spanningTree != null)
						{
							Console.WriteLine(Yellow("Prim Spanning Tree: "));
							spanningTree.Print();
							Console.WriteLine(LINE_SEPARATOR);
						}
						
						spanningTree = weightedUndirectedGraph.GetMinimumSpanningTree(SpanningTreeAlgorithm.Kruskal);

						if (spanningTree != null)
						{
							Console.WriteLine(Yellow("Kruskal Spanning Tree: "));
							spanningTree.Print();
							Console.WriteLine(LINE_SEPARATOR);
						}
						break;
					case WeightedMixedGraphList<char, int> weightedMixedGraph:
						Console.WriteLine(Yellow("InDegree: ") + weightedMixedGraph.InDegree(value));
						try { Console.WriteLine(Yellow("Topological Sort: ") + string.Join(", ", weightedMixedGraph.TopologicalSort())); }
						catch (Exception e) { Console.WriteLine(Yellow("Topological Sort: ") + Bright.Red(e.Message)); }
						break;
				}

				if (graph is WeightedGraphList<char, int> wGraph)
				{
					char to = IListExtension.PickRandom(values);
					ConsoleKeyInfo response;

					do
					{
						Console.Write($@"Current position is '{Bright.Green(value.ToString())}'. Type in {Bright.Green("a character")} to find the shortest path to,
(You can press the {Bright.Green("RETURN")} key to accept the current random value '{Bright.Green(to.ToString())}'),
or press {Bright.Red("ESCAPE")} key to exit this test. ");
						response = Console.ReadKey();
						Console.WriteLine();
						if (response.Key == ConsoleKey.Escape) return false;
						if (response.Key == ConsoleKey.Enter) continue;
						if (!char.IsLetter(response.KeyChar) || !wGraph.ContainsKey(response.KeyChar)) Console.WriteLine($"Character '{value}' is not found or not connected!");
						to = response.KeyChar;
						break;
					}
					while (response.Key != ConsoleKey.Enter);

					Console.WriteLine();
					Console.WriteLine($"{Yellow("Shortest Path")} from '{Bright.Cyan(value.ToString())}' to '{Bright.Cyan(to.ToString())}'");
					
					Console.Write("Dijkstra: ");
					try { Console.WriteLine(string.Join(" -> ", wGraph.SingleSourcePath(value, to, SingleSourcePathAlgorithm.Dijkstra))); }
					catch (Exception e) { Console.WriteLine(Bright.Red(e.Message)); }
					
					Console.Write("Bellman-Ford: ");
					try { Console.WriteLine(string.Join(" -> ", wGraph.SingleSourcePath(value, to, SingleSourcePathAlgorithm.BellmanFord))); }
					catch (Exception e) { Console.WriteLine(Bright.Red(e.Message)); }
				}

				return true;
			}
		}

		private static void TestAsymmetric()
		{
			RSASettings settings = new RSASettings {
				Encoding = Encoding.UTF8,
				KeySize = 512,
				SaltSize = 8,
				Padding = RSAEncryptionPadding.Pkcs1,
				UseExpiration = false
			};
			(string publicKey, string privateKey) = QuickCipher.GenerateAsymmetricKeys(false, settings);
			Title("Generated keys");
			Console.WriteLine($@"Public:
'{publicKey}'

Private:
'{privateKey}'
");

			string data = "This is test data.";
			string encrypted = QuickCipher.AsymmetricEncrypt(publicKey, data, settings);
			SecureString decrypted = QuickCipher.AsymmetricDecrypt(privateKey, encrypted, settings);
			Title("Encrypted");
			Console.WriteLine($@"data:
'{data}'

encrypted:
'{encrypted}'

decrypted:
'{decrypted.UnSecure()}'");
		}

		private static void TestSingletonAppGuard()
		{
			SingletonAppGuard guard = null;

			try
			{
				guard = new SingletonAppGuard(1000);
				Console.WriteLine("Heellloooo.!");
				Console.WriteLine("Sleeping for a while...");
				Thread.Sleep(5000);
			}
			catch (TimeoutException)
			{
				Console.WriteLine("Can't run! Another instance is running...");
			}
			finally
			{
				ObjectHelper.Dispose(ref guard);
			}
		}

		private static void TestImpersonationHelper()
		{
			const string SERVICE_NAME = "BITS";

			Title("Testing ImpersonationHelper...");

			bool elevated = WindowsIdentityHelper.HasElevatedPrivileges();
			Console.WriteLine($"Current process is running with elevated privileges? {elevated.ToYesNo()}");

			Console.WriteLine($"Checking {SERVICE_NAME} service status...");
			ServiceController controller = null;

			try
			{
				controller = ServiceControllerHelper.GetController(SERVICE_NAME);
				Console.WriteLine($"Is running? {controller.IsRunning().ToYesNo()}");
			}
			finally
			{
				ObjectHelper.Dispose(ref controller);
			}
		}

		private static void TestServiceHelper()
		{
			const string SERVICE_NAME = "MSMQ";

			TimeSpan timeout = TimeSpanHelper.ThirtySeconds;
			Action<ServiceController> startService = sc =>
			{
				sc.Start();
				sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
			};

			Action<ServiceController> stopService = sc =>
			{
				sc.Stop();
				sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
			};

			Title($"Testing ServiceControllerHelper with service {SERVICE_NAME}...");

			ServiceController controller = null;
			WindowsIdentity identity = null;

			try
			{
				controller = ServiceControllerHelper.GetController(SERVICE_NAME);
				Console.WriteLine($"Is running? {controller.IsRunning().ToYesNo()}");
				Console.WriteLine("Asserting access rights...");

				identity = WindowsIdentity.GetCurrent();

				bool canControl = identity.User != null && controller.AssertControlAccessRights(identity.User);
				Console.WriteLine($"Can control service? {canControl.ToYesNo()}");

				if (canControl)
				{
					if (controller.IsRunning())
					{
						Console.WriteLine("Stopping the service...");
						controller.InvokeWithElevatedPrivilege(stopService);
						Console.WriteLine("Sleeping for a while...");
						Thread.Sleep(3000);
					}

					if (controller.IsStopped())
					{
						Console.WriteLine("Starting the service...");
						controller.InvokeWithElevatedPrivilege(startService);
					}
				}
			}
			catch (InvalidOperationException iox) when (iox.InnerException != null)
			{
				Console.WriteLine("I'm not running with elevated privilege! Would you like to restart me as an admin? [Y / Any key]");
				if (Console.ReadKey().Key != ConsoleKey.Y) return;

				//ProcessStartInfo startInfo = new ProcessStartInfo(AppInfo.ExecutablePath)
				//{
				//	UseShellExecute = true,
				//	Verb = "runas",
				//	WindowStyle = ProcessWindowStyle.Normal
				//};
				//Process.Start(startInfo);

				ProcessHelper.ShellExec(AppInfo.ExecutablePath, new ShellSettings
				{
					WorkingDirectory = AppInfo.Directory,
					Verb = "runas"
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(Red(ex.CollectMessages()));
			}
			finally
			{
				ObjectHelper.Dispose(ref controller);
				ObjectHelper.Dispose(ref identity);
			}
		}

		private static void TestUriHelper()
		{
			const string URI_TEST = "http://example.com/folder path";
			
			string[] uriParts =
			{
				"/another folder",
				"more_folders/folder 2",
				"image file.jpg"
			};

			Uri baseUri = UriHelper.ToUri(URI_TEST, UriKind.Absolute);
			Console.WriteLine($"{URI_TEST} => {baseUri.String()}");

			Uri uri = new Uri(baseUri.ToString());

			foreach (string part in uriParts)
			{
				Uri newUri = UriHelper.Combine(uri, part);
				Console.WriteLine($"{uri.String()} + {part} => {newUri.String()}");
				uri = newUri;
			}

			uri = UriHelper.ToUri(uriParts[0]);
			Console.WriteLine($"{uriParts[0]} => {uri.String()}");

			uri = UriHelper.ToUri(uriParts[1]);
			Console.WriteLine($"{uriParts[1]} => {uri.String()}");

			string[] urls = {
				"server:8088",
				"server:8088/func1",
				"server:8088/func1/SubFunc1",
				"server:8088/my folder/my image.jpg",
				"http://server",
				"http://server/func1",
				"http://server/func/SubFunc1",
				"http://server:8088",
				"http://server:8088/func1",
				"http://server:8088/func1/SubFunc1",
				"magnet://server",
				"magnet://server/func1",
				"magnet://server/func/SubFunc1",
				"magnet://server:8088",
				"magnet://server:8088/func1",
				"magnet://server:8088/func1/SubFunc1",
				"http://[2001:db8::1]",
				"http://[2001:db8::1]:80",
			};

			foreach (string item in urls)
			{
				uri = UriHelper.ToUri(item);
				Console.WriteLine(uri.String());
			}
		}

		private static void TestUriHelperRelativeUrl()
		{
			const string BASE_URI = "/files/images/users";
			Uri relUri = UriHelper.Combine(BASE_URI, Guid.NewGuid().ToString(), "auto_f_92.jpg");
			Console.WriteLine(relUri.String());
		}

		private static void TestJsonUriConverter()
		{
			string[] urls = {
				"server:8088",
				"server:8088/func1",
				"server:8088/func1/SubFunc1",
				"server:8088/my folder/my image.jpg",
				"http://server",
				"http://server/func1",
				"http://server/func/SubFunc1",
				"http://server:8088",
				"http://server:8088/func1",
				"http://server:8088/func1/SubFunc1",
				"magnet://server",
				"magnet://server/func1",
				"magnet://server/func/SubFunc1",
				"magnet://server:8088",
				"magnet://server:8088/func1",
				"magnet://server:8088/func1/SubFunc1",
				"http://[2001:db8::1]",
				"http://[2001:db8::1]:80",
			};

			JsonSerializerSettings settings = JsonHelper.CreateSettings().AddConverters();
			UriTestClass uriTest = new UriTestClass();

			foreach (string item in urls)
			{
				uriTest.Uri = UriHelper.ToUri(item);
				Console.WriteLine($"{item} => {JsonConvert.SerializeObject(uriTest, settings)}");
			}
		}

		private static void TestDevicesMonitor()
		{
			TestUSBForm form = null;
			Title("Testing devices monitor");

			try
			{
				form = new TestUSBForm();
				form.ShowDialog();
			}
			catch (Exception ex)
			{
				Console.WriteLine(Bright.Red(ex.Message));
			}
			finally
			{
				ObjectHelper.Dispose(ref form);
			}
		}

		private static void Title(string title)
		{
			Console.WriteLine();
			Console.WriteLine(Bold().Bright.Black(title));
			Console.WriteLine();
		}

		private static void CompilationHint()
		{
			Console.WriteLine(__compilationText);
			Console.WriteLine();
		}

		[NotNull]
		private static Action<IList<T>, int, int, IComparer<T>, bool> GetAlgorithm<T>([NotNull] string name)
		{
			return name switch
			{
				nameof(IListExtension.SortBubble) => IListExtension.SortBubble,
				nameof(IListExtension.SortSelection) => IListExtension.SortSelection,
				nameof(IListExtension.SortInsertion) => IListExtension.SortInsertion,
				nameof(IListExtension.SortHeap) => IListExtension.SortHeap,
				nameof(IListExtension.SortMerge) => IListExtension.SortMerge,
				nameof(IListExtension.SortQuick) => IListExtension.SortQuick,
				nameof(IListExtension.SortShell) => IListExtension.SortShell,
				nameof(IListExtension.SortComb) => IListExtension.SortComb,
				nameof(IListExtension.SortTim) => IListExtension.SortTim,
				nameof(IListExtension.SortCocktail) => IListExtension.SortCocktail,
				nameof(IListExtension.SortBitonic) => IListExtension.SortBitonic,
				nameof(IListExtension.SortPancake) => IListExtension.SortPancake,
				nameof(IListExtension.SortBinary) => IListExtension.SortBinary,
				nameof(IListExtension.SortGnome) => IListExtension.SortGnome,
				nameof(IListExtension.SortBrick) => IListExtension.SortBrick,
				_ => throw new NotFoundException()
			};
		}

		private static bool RandomBool() { return RandomHelper.Default.Next(1) == 1; }

		[NotNull]
		private static int[] GetRandomIntegers(int len = 0) { return GetRandomIntegers(false, len); }
		
		[NotNull]
		private static int[] GetRandomIntegers(bool unique, int len = 0)
		{
			const double GAPS_THRESHOLD = 0.25d;

			if (len < 1) len = RNGRandomHelper.Next(1, 12);

			int[] values = new int[len];

			if (unique)
			{
				int gaps = (int)(len * GAPS_THRESHOLD);
				values = Enumerable.Range(1, len).ToArray();

				int min = len + 1, max = min + gaps + 1;

				for (int i = 0; i < gaps; i++)
					values[RNGRandomHelper.Next(0, values.Length - 1)] = RNGRandomHelper.Next(min, max);

				values.Shuffle();
			}
			else
			{
				for (int i = 0; i < len; i++)
					values[i] = RNGRandomHelper.Next(1, short.MaxValue);
			}

			return values;
		}

		[NotNull]
		private static char[] GetRandomChar(int len = 0) { return GetRandomChar(false, len); }
		
		[NotNull]
		private static char[] GetRandomChar(bool unique, int len = 0)
		{
			if (len < 1) len = RNGRandomHelper.Next(1, 12);
			
			char[] values = new char[len];

			if (unique)
			{
				int i = 0;
				HashSet<char> set = new HashSet<char>();

				while (i < len)
				{
					char value = (char)RNGRandomHelper.Next('a', 'z');
					if (!set.Add(value)) continue;
					values[i++] = value;
				}
			}
			else
			{
				for (int i = 0; i < len; i++)
				{
					values[i] = (char)RNGRandomHelper.Next('a', 'z');
				}
			}

			return values;
		}

		[NotNull]
		private static ICollection<string> GetRandomStrings(int len = 0) { return GetRandomStrings(false, len); }
		
		[NotNull]
		private static ICollection<string> GetRandomStrings(bool unique, int len = 0)
		{
			if (len < 1) len = RNGRandomHelper.Next(1, 12);
			if (!unique) return __fakeGenerator.Value.Random.WordsArray(len);

			HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			while (set.Count < len) 
				set.Add(__fakeGenerator.Value.Random.Word());

			return set;
		}

		[NotNull]
		private static Student[] GetRandomStudents(int len = 0)
		{
			if (len < 1) len = RNGRandomHelper.Next(1, 12);
			
			Student[] students = new Student[len];

			for (int i = 0; i < len; i++)
			{
				students[i] = new Student
				{
					Name = __fakeGenerator.Value.Name.FirstName(__fakeGenerator.Value.PickRandom<Name.Gender>()),
					Grade = __fakeGenerator.Value.Random.Double(0.0d, 100.0d)
				};
			}

			return students;
		}

		private static bool LimitThreads()
		{
			// change this to true to use 1 thread only for debugging
			return true;
		}
	}
}
