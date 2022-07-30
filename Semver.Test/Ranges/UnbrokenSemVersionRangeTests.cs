﻿using System;
using Semver.Ranges;
using Semver.Test.Builders;
using Xunit;

namespace Semver.Test.Ranges
{
    public class UnbrokenSemVersionRangeTests
    {
        private const string MaxVersion = "2147483647.2147483647.2147483647";
        private const string MinVersion = "0.0.0-0";
        private const string MinReleaseVersion = "0.0.0";
        private static readonly SemVersion VersionWithMetadata =
            SemVersion.Parse("1.2.3-foo+metadata", SemVersionStyles.Strict);
        private static readonly SemVersion FakeVersion = new SemVersion(1, 2, 3);

        [Fact]
        public void MinVersionIsZeroWithZeroPrerelease()
        {
            Assert.Equal(SemVersion.Parse("0.0.0-0", SemVersionStyles.Strict), SemVersion.Min);
        }

        [Fact]
        public void MinReleaseVersionIsZero()
        {
            Assert.Equal(new SemVersion(0), SemVersion.MinRelease);
        }

        [Fact]
        public void MaxVersionIsIntMax()
        {
            Assert.Equal(new SemVersion(int.MaxValue, int.MaxValue, int.MaxValue), SemVersion.Max);
        }

        [Fact]
        public void EmptyIsMaximallyEmpty()
        {
            var empty = UnbrokenSemVersionRange.Empty;
            Assert.Equal(SemVersion.Max, empty.Start);
            Assert.False(empty.StartInclusive, "Start Inclusive");
            Assert.Equal(SemVersion.Min, empty.End);
            Assert.False(empty.EndInclusive, "End Inclusive");
            Assert.False(empty.IncludeAllPrerelease, "Include All Prerelease");
        }

        public static readonly TheoryData<string> FullRangeOfVersions = new TheoryData<string>
        {
            MinVersion,
            MinReleaseVersion,
            "1.2.3",
            "4.5.6-rc",
            "7.8.9-beta.0",
            "2147483647.2147483647.2147483647-alpha",
            MaxVersion,
        };

        [Theory]
        [MemberData(nameof(FullRangeOfVersions))]
        public void EmptyDoesNotContain(string version)
        {
            Assert.False(UnbrokenSemVersionRange.Empty.Contains(SemVersion.Parse(version, SemVersionStyles.Strict)));
        }

        [Fact]
        public void AllReleaseProperties()
        {
            var allRelease = UnbrokenSemVersionRange.AllRelease;
            Assert.Null(allRelease.Start);
            Assert.False(allRelease.StartInclusive, "Start Inclusive");
            Assert.Equal(SemVersion.Max, allRelease.End);
            Assert.True(allRelease.EndInclusive, "End Inclusive");
            Assert.False(allRelease.IncludeAllPrerelease, "Include All Prerelease");
        }

        [Theory]
        [MemberData(nameof(FullRangeOfVersions))]
        public void AllReleaseContains(string version)
        {
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(!semVersion.IsPrerelease, UnbrokenSemVersionRange.AllRelease.Contains(semVersion));
        }

        [Fact]
        public void AllProperties()
        {
            var all = UnbrokenSemVersionRange.All;
            Assert.Null(all.Start);
            Assert.False(all.StartInclusive, "Start Inclusive");
            Assert.Equal(SemVersion.Max, all.End);
            Assert.True(all.EndInclusive, "End Inclusive");
            Assert.True(all.IncludeAllPrerelease, "Include All Prerelease");
        }

        [Theory]
        [MemberData(nameof(FullRangeOfVersions))]
        public void AllContains(string version)
        {
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.True(UnbrokenSemVersionRange.All.Contains(semVersion));
        }

        [Fact]
        public void EqualsNullVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.Equals(null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Fact]
        public void EqualsMetadataVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.Equals(VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void EqualsContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Equals(new SemVersion(1, 2, 3));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void EqualsPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Equals(SemVersion.ParsedFrom(1, 2, 3, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void GreaterThanNullVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.GreaterThan(null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Fact]
        public void GreaterThanMetadataVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.GreaterThan(VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("2.0.0-pre", false)]
        [InlineData(MaxVersion, true)]
        public void GreaterThanContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.GreaterThan(new SemVersion(1, 2, 3));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", true)]
        [InlineData("1.2.4", true)]
        [InlineData("2.0.0-pre", true)]
        [InlineData(MaxVersion, true)]
        public void GreaterThanIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.GreaterThan(new SemVersion(1, 2, 3),
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("2.0.0-pre", false)]
        [InlineData(MaxVersion, true)]
        public void GreaterThanPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.GreaterThan(SemVersion.ParsedFrom(1, 2, 3, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void AtLeastNullVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.AtLeast(null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Fact]
        public void AtLeastMetadataVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.AtLeast(VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("2.0.0-pre", false)]
        [InlineData(MaxVersion, true)]
        public void AtLeastContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.AtLeast(new SemVersion(1, 2, 3));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", true)]
        [InlineData("1.2.4", true)]
        [InlineData("2.0.0-pre", true)]
        [InlineData(MaxVersion, true)]
        public void AtLeastIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.AtLeast(new SemVersion(1, 2, 3), includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("2.0.0-pre", false)]
        [InlineData(MaxVersion, true)]
        public void AtLeastPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.AtLeast(SemVersion.ParsedFrom(1, 2, 3, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void LessThanNullVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.LessThan(null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Fact]
        public void LessThanMetadataVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.LessThan(VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, true)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", true)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void LessThanContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.LessThan(new SemVersion(1, 2, 3));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, true)]
        [InlineData(MinReleaseVersion, true)]
        [InlineData("1.2.2-pre", true)]
        [InlineData("1.2.2", true)]
        [InlineData("1.2.3-4", true)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void LessThanIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.LessThan(new SemVersion(1, 2, 3),
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, true)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", true)]
        [InlineData("1.2.3-4", true)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void LessThanPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.LessThan(SemVersion.ParsedFrom(1, 2, 3, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void AtMostNullVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.AtMost(null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Fact]
        public void AtMostMetadataVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.AtMost(VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("version", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, true)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", true)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void AtMostContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.AtMost(new SemVersion(1, 2, 3));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, true)]
        [InlineData(MinReleaseVersion, true)]
        [InlineData("1.2.2-pre", true)]
        [InlineData("1.2.2", true)]
        [InlineData("1.2.3-4", true)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void AtMostIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.AtMost(new SemVersion(1, 2, 3), includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, true)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", true)]
        [InlineData("1.2.3-4", true)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", false)]
        [InlineData(MaxVersion, false)]
        public void AtMostPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.AtMost(SemVersion.ParsedFrom(1, 2, 3, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void InclusiveNullStartVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.Inclusive(null, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void InclusiveMetadataStartVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.Inclusive(VersionWithMetadata, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void InclusiveNullEndVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.Inclusive(FakeVersion, null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Fact]
        public void InclusiveMetadataEndVersion()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => UnbrokenSemVersionRange.Inclusive(FakeVersion, VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", false)]
        [InlineData("4.5.6-5", false)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", true)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Inclusive(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", true)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", true)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", true)]
        [InlineData("4.5.6-6", true)]
        [InlineData("4.5.6", true)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Inclusive(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6),
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", true)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusivePrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Inclusive(SemVersion.ParsedFrom(1, 2, 3, "5"), SemVersion.ParsedFrom(4, 5, 6, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void InclusiveOfStartNullStartVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.InclusiveOfStart(null, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void InclusiveOfStartMetadataStartVersion()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => UnbrokenSemVersionRange.InclusiveOfStart(VersionWithMetadata, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void InclusiveOfStartNullEndVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.InclusiveOfStart(FakeVersion, null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Fact]
        public void InclusiveOfStartMetadataEndVersion()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => UnbrokenSemVersionRange.InclusiveOfStart(FakeVersion, VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", false)]
        [InlineData("4.5.6-5", false)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveOfStartContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.InclusiveOfStart(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", true)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", true)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", true)]
        [InlineData("4.5.6-6", true)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveOfStartIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.InclusiveOfStart(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6),
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", true)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", false)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveOfStartPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.InclusiveOfStart(SemVersion.ParsedFrom(1, 2, 3, "5"), SemVersion.ParsedFrom(4, 5, 6, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void InclusiveOfEndNullStartVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.InclusiveOfEnd(null, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void InclusiveOfEndMetadataStartVersion()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => UnbrokenSemVersionRange.InclusiveOfEnd(VersionWithMetadata, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void InclusiveOfEndNullEndVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.InclusiveOfEnd(FakeVersion, null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Fact]
        public void InclusiveOfEndMetadataEndVersion()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => UnbrokenSemVersionRange.InclusiveOfEnd(FakeVersion, VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", false)]
        [InlineData("4.5.6-5", false)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", true)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveOfEndContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.InclusiveOfEnd(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", true)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", true)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", true)]
        [InlineData("4.5.6-6", true)]
        [InlineData("4.5.6", true)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveOfEndIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.InclusiveOfEnd(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6),
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", true)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void InclusiveOfEndPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.InclusiveOfEnd(SemVersion.ParsedFrom(1, 2, 3, "5"), SemVersion.ParsedFrom(4, 5, 6, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void ExclusiveNullStartVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.Exclusive(null, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void ExclusiveMetadataStartVersion()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => UnbrokenSemVersionRange.Exclusive(VersionWithMetadata, FakeVersion));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("start", ex.ParamName);
        }

        [Fact]
        public void ExclusiveNullEndVersion()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => UnbrokenSemVersionRange.Exclusive(FakeVersion, null));
            Assert.StartsWith(ExceptionMessages.NotNull, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Fact]
        public void ExclusiveMetadataEndVersion()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => UnbrokenSemVersionRange.Exclusive(FakeVersion, VersionWithMetadata));
            Assert.StartsWith(ExceptionMessages.NoMetadata, ex.Message);
            Assert.Equal("end", ex.ParamName);
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", false)]
        [InlineData("4.5.6-5", false)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void ExclusiveContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Exclusive(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.4-0", true)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", true)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", true)]
        [InlineData("4.5.6-6", true)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void ExclusiveIncludingAllPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Exclusive(new SemVersion(1, 2, 3), new SemVersion(4, 5, 6),
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Theory]
        [InlineData(MinVersion, false)]
        [InlineData(MinReleaseVersion, false)]
        [InlineData("1.2.2-pre", false)]
        [InlineData("1.2.2", false)]
        [InlineData("1.2.3-4", false)]
        [InlineData("1.2.3-5", false)]
        [InlineData("1.2.3-6", true)]
        [InlineData("1.2.3", true)]
        [InlineData("1.2.4-0", false)]
        [InlineData("1.2.4", true)]
        [InlineData("4.5.5-0", false)]
        [InlineData("4.5.5", true)]
        [InlineData("4.5.6-4", true)]
        [InlineData("4.5.6-5", false)]
        [InlineData("4.5.6-6", false)]
        [InlineData("4.5.6", false)]
        [InlineData("4.5.7-0", false)]
        [InlineData("4.5.7", false)]
        [InlineData(MaxVersion, false)]
        public void ExclusivePrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.Exclusive(SemVersion.ParsedFrom(1, 2, 3, "5"), SemVersion.ParsedFrom(4, 5, 6, "5"));
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void LessThanMinReleaseVersionEmpty()
        {
            var range = UnbrokenSemVersionRange.LessThan(SemVersion.MinRelease);
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }

        [Theory]
        [InlineData(MinVersion, true)]
        [InlineData("0.0.0-Z", true)]
        [InlineData("0.0.0", false)]
        public void LessThanMinReleaseVersionIncludingPrereleaseContains(string version, bool expected)
        {
            var range = UnbrokenSemVersionRange.LessThan(SemVersion.MinRelease,
                            includeAllPrerelease: true);
            var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);
            Assert.Equal(expected, range.Contains(semVersion));
        }

        [Fact]
        public void LessThanMinVersionEmpty()
        {
            var range = UnbrokenSemVersionRange.LessThan(SemVersion.Min,
                            includeAllPrerelease: true);
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }

        [Fact]
        public void InclusiveEmpty()
        {
            var range = UnbrokenSemVersionRange.Inclusive(new SemVersion(1, 2, 3), new SemVersion(1, 2, 2),
                            includeAllPrerelease: true);
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }

        [Fact]
        public void InclusiveWithPrereleaseEmpty()
        {
            var range = UnbrokenSemVersionRange.Inclusive(new SemVersion(1, 2, 3), new SemVersion(1, 2, 2),
                            includeAllPrerelease: true);
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }

        [Fact]
        public void ExclusiveStartEqualsEndEmpty()
        {
            var range = UnbrokenSemVersionRange.Exclusive(new SemVersion(1, 2, 3), new SemVersion(1, 2, 3));
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }

        [Fact]
        public void ExclusiveEmpty()
        {
            var range = UnbrokenSemVersionRange.Exclusive(new SemVersion(1, 2, 3), new SemVersion(1, 2, 4));
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }

        [Fact]
        public void ExclusiveAtMaxEmpty()
        {
            var range = UnbrokenSemVersionRange.Exclusive(SemVersion.Max, SemVersion.Max);
            Assert.Equal(UnbrokenSemVersionRange.Empty, range);
        }
    }
}