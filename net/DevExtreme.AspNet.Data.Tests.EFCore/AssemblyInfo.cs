using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

#if EFCORE3 || EFCORE5
#warning TODO fix coverage report, https://github.com/OpenCover/opencover/issues/894
#endif
