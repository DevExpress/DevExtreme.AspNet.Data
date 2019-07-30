using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

#if EFCORE3
#warning TODO fix coverage report, https://github.com/OpenCover/opencover/issues/894
#endif
