using System.Diagnostics;

namespace GameServer.Core.Tests;

public class DictVsArrayTests
{
    private static readonly int[] ElementCounts =
    [
        10,
        100,
        1_000,
        10_000,
        100_000,
        1_000_000,
    ];

    private const int QueriesPerRound = 1_000_000;
    private const int MeasurementRounds = 5;

    private struct TestStruct
    {
        public int id;
        public string name;
    }

    [Test]
    public void QuerySpeedAtDifferentElementCounts()
    {
        TestContext.Out.WriteLine(
            "{0,12} {1,14} {2,18} {3,18} {4,14}",
            "Elements",
            "Queries",
            "Array(ns/query)",
            "Dict(ns/query)",
            "Dict/Array");

        foreach (int elementCount in ElementCounts)
        {
            TestStruct[] array = new TestStruct[elementCount];
            Dictionary<int, TestStruct> dictionary = new(elementCount);
            int[] queryIds = new int[elementCount];

            for (int i = 0; i < elementCount; i++)
            {
                TestStruct value = new()
                {
                    id = i,
                    name = "test",
                };

                array[i] = value;
                dictionary.Add(i, value);
                queryIds[i] = i;
            }

            new Random(42).Shuffle(queryIds);

            int repetitions = QueriesPerRound / elementCount;
            int warmupRepetitions = Math.Max(1, 10_000 / elementCount);
            QueryArray(array, queryIds, warmupRepetitions);
            QueryDictionary(dictionary, queryIds, warmupRepetitions);

            long arrayElapsedTicks = 0;
            long dictionaryElapsedTicks = 0;

            for (int round = 0; round < MeasurementRounds; round++)
            {
                long arrayChecksum;
                long dictionaryChecksum;

                if ((round & 1) == 0)
                {
                    (long elapsedTicks, long checksum) arrayResult =
                        MeasureArrayQueries(array, queryIds, repetitions);
                    (long elapsedTicks, long checksum) dictionaryResult =
                        MeasureDictionaryQueries(dictionary, queryIds, repetitions);

                    arrayElapsedTicks += arrayResult.elapsedTicks;
                    dictionaryElapsedTicks += dictionaryResult.elapsedTicks;
                    arrayChecksum = arrayResult.checksum;
                    dictionaryChecksum = dictionaryResult.checksum;
                }
                else
                {
                    (long elapsedTicks, long checksum) dictionaryResult =
                        MeasureDictionaryQueries(dictionary, queryIds, repetitions);
                    (long elapsedTicks, long checksum) arrayResult =
                        MeasureArrayQueries(array, queryIds, repetitions);

                    dictionaryElapsedTicks += dictionaryResult.elapsedTicks;
                    arrayElapsedTicks += arrayResult.elapsedTicks;
                    dictionaryChecksum = dictionaryResult.checksum;
                    arrayChecksum = arrayResult.checksum;
                }

                Assert.That(
                    arrayChecksum,
                    Is.EqualTo(dictionaryChecksum),
                    $"Query result differs at element count {elementCount}");
            }

            long totalQueries = (long)elementCount * repetitions * MeasurementRounds;
            double arrayNanosecondsPerQuery =
                arrayElapsedTicks * 1_000_000_000.0 / Stopwatch.Frequency / totalQueries;
            double dictionaryNanosecondsPerQuery =
                dictionaryElapsedTicks * 1_000_000_000.0 / Stopwatch.Frequency / totalQueries;

            TestContext.Out.WriteLine(
                "{0,12:N0} {1,14:N0} {2,18:F2} {3,18:F2} {4,14:F2}x",
                elementCount,
                totalQueries,
                arrayNanosecondsPerQuery,
                dictionaryNanosecondsPerQuery,
                dictionaryNanosecondsPerQuery / arrayNanosecondsPerQuery);
        }
    }

    private static (long elapsedTicks, long checksum) MeasureArrayQueries(
        TestStruct[] array,
        int[] queryIds,
        int repetitions)
    {
        long start = Stopwatch.GetTimestamp();
        long checksum = QueryArray(array, queryIds, repetitions);
        return (Stopwatch.GetTimestamp() - start, checksum);
    }

    private static (long elapsedTicks, long checksum) MeasureDictionaryQueries(
        Dictionary<int, TestStruct> dictionary,
        int[] queryIds,
        int repetitions)
    {
        long start = Stopwatch.GetTimestamp();
        long checksum = QueryDictionary(dictionary, queryIds, repetitions);
        return (Stopwatch.GetTimestamp() - start, checksum);
    }

    private static long QueryArray(TestStruct[] array, int[] queryIds, int repetitions)
    {
        long checksum = 0;

        for (int repetition = 0; repetition < repetitions; repetition++)
        {
            for (int i = 0; i < queryIds.Length; i++)
            {
                checksum += array[queryIds[i]].id;
            }
        }

        return checksum;
    }

    private static long QueryDictionary(
        Dictionary<int, TestStruct> dictionary,
        int[] queryIds,
        int repetitions)
    {
        long checksum = 0;

        for (int repetition = 0; repetition < repetitions; repetition++)
        {
            for (int i = 0; i < queryIds.Length; i++)
            {
                checksum += dictionary[queryIds[i]].id;
            }
        }

        return checksum;
    }
}
