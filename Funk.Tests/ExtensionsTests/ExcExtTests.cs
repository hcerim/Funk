using System;
using System.Threading.Tasks;
using Xunit;

namespace Funk.Tests
{
    public class ExcExtTests : Test
    {
        [Fact]
        public void AsSuccess_On_Successful_Exc_Returns_NonEmpty_Maybe()
        {
            UnitTest(
                _ => success<string, Exception>("Funk"),
                e => e.AsSuccess(),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.Equal("Funk", m.UnsafeGet());
                }
            );
        }

        [Fact]
        public void AsSuccess_On_Failed_Exc_Returns_Empty_Maybe()
        {
            UnitTest(
                _ => failure<string, Exception>(new Exception("error")),
                e => e.AsSuccess(),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void AsSuccess_On_Empty_Exc_Returns_Empty_Maybe()
        {
            UnitTest(
                _ => Exc.Empty<string, Exception>(),
                e => e.AsSuccess(),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Flatten_Nested_Exc()
        {
            UnitTest(
                _ => success<Exc<string, Exception>, Exception>(success<string, Exception>("Funk")),
                e => e.Flatten(),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("Funk", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void MapFailure_Transforms_Exception_Type()
        {
            UnitTest(
                _ => failure<string, ArgumentException>(new ArgumentException("arg error")),
                e => e.MapFailure(f => new InvalidOperationException(f.Root.UnsafeGet().Message)),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.IsType<EnumerableException<InvalidOperationException>>(r.Failure.UnsafeGet());
                    Assert.Equal("arg error", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public async Task MapFailureAsync_Transforms_Exception_Type()
        {
            await UnitTestAsync(
                _ => result(failure<string, ArgumentException>(new ArgumentException("async error"))),
                e => e.MapFailureAsync(f => result(new InvalidOperationException(f.Root.UnsafeGet().Message))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.IsType<EnumerableException<InvalidOperationException>>(r.Failure.UnsafeGet());
                    Assert.Equal("async error", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void OnFailure_Recovers_From_Failure()
        {
            UnitTest(
                _ => failure<string, Exception>(new Exception("failed")),
                e => e.OnFailure(_ => "recovered"),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("recovered", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void OnFlatFailure_Recovers_From_Failure_With_Exc()
        {
            UnitTest(
                _ => failure<string, Exception>(new Exception("failed")),
                e => e.OnFlatFailure(_ => success<string, Exception>("recovered")),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("recovered", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public async Task OnFailureAsync_Recovers_From_Failure()
        {
            await UnitTestAsync(
                _ => result(failure<string, Exception>(new Exception("failed"))),
                e => e.OnFailureAsync(_ => result("recovered")),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("recovered", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void OnEmpty_Recovers_From_Empty()
        {
            UnitTest(
                _ => Exc.Empty<string, Exception>(),
                e => e.OnEmpty(_ => "recovered"),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("recovered", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void OnFlatEmpty_Recovers_From_Empty_With_Exc()
        {
            UnitTest(
                _ => Exc.Empty<string, Exception>(),
                e => e.OnFlatEmpty(_ => success<string, Exception>("recovered")),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("recovered", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public async Task OnEmptyAsync_Recovers_From_Empty()
        {
            await UnitTestAsync(
                _ => result(Exc.Empty<string, Exception>()),
                e => e.OnEmptyAsync(_ => result("recovered")),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("recovered", r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void Merge_Two_Successful_Excs()
        {
            UnitTest(
                _ => success<int, Exception>(1),
                e => e.Merge(success<int, Exception>(2)),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal(2, r.Success.UnsafeGet().Count);
                    Assert.Equal(1, r.Success.UnsafeGet()[0]);
                    Assert.Equal(2, r.Success.UnsafeGet()[1]);
                }
            );
        }

        [Fact]
        public void Merge_Successful_With_Failed_Exc()
        {
            UnitTest(
                _ => success<string, Exception>("ok"),
                e => e.Merge(failure<string, Exception>(new Exception("fail"))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.Equal(1, r.NestedFailures.UnsafeGet().Count);
                }
            );
        }

        [Fact]
        public void MergeRange_Multiple_Excs()
        {
            UnitTest(
                _ => success<string, Exception>("a"),
                e => e.MergeRange([success<string, Exception>("b"), success<string, Exception>("c")]),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal(3, r.Success.UnsafeGet().Count);
                }
            );
        }

        [Fact]
        public async Task MapAsync_On_TaskExc_Chains()
        {
            await UnitTestAsync(
                _ => result(success<string, Exception>("Funk")),
                e => e.MapAsync(s => result(s.Length)),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal(4, r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public async Task FlatMapAsync_On_TaskExc_Chains()
        {
            await UnitTestAsync(
                _ => result(success<string, Exception>("Funk")),
                e => e.FlatMapAsync(s => result(success<int, Exception>(s.Length))),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal(4, r.UnsafeGetSuccess());
                }
            );
        }
    }
}
