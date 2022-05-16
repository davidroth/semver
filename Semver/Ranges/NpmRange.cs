using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Semver.Ranges.Comparers.Npm;

namespace Semver.Ranges
{
    /// <summary>
    /// A range of versions that can be checked against to see if a <see cref="SemVersion"/> is included.
    /// Uses the same syntax as npm.
    /// </summary>
    public class NpmRange : ISemVersionRange
    {
        internal readonly NpmComparator[][] Ranges;
        private string cachedStringValue;

        internal NpmRange(IEnumerable<NpmComparator[]> comparators)
        {
            if (comparators is NpmComparator[][] arrayComparators)
                Ranges = arrayComparators;
            else
                Ranges = comparators.ToArray();

            if (Ranges.Length == 0)
                throw new ArgumentException("There must be atleast one comparator in the range");
        }

        /// <summary>
        /// Parses the range.
        /// </summary>
        /// <param name="range">The range to parse.</param>
        /// <returns>The parsed range</returns>
        /// <exception cref="ArgumentNullException">Thrown when range is null.</exception>
        /// <exception cref="FormatException">Thrown when the range has invalid syntax or if regex match timed out.</exception>
        public static NpmRange Parse(string range)
        {
            return Parse(range, NpmParseOptions.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range">The range to parse.</param>
        /// <param name="options">The options to use when parsing.</param>
        /// <returns>The parsed range.</returns>
        /// <exception cref="ArgumentNullException">Thrown when range or options is null.</exception>
        /// <exception cref="FormatException">Thrown when the range has invalid syntax or if regex match timed out.</exception>
        public static NpmRange Parse(string range, NpmParseOptions options)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (options == null) throw new ArgumentNullException(nameof(options));

            try
            {
                return new RangeParser().ParseRange(range, options);
            }
            catch (RegexMatchTimeoutException ex)
            {
                throw new FormatException("Regex match timed out", ex);
            }
        }

        /// <summary>
        /// Tries to parse the range and returns true if successful.
        /// </summary>
        /// <param name="strRange">The range to parse.</param>
        /// <param name="range">The parsed <see cref="NpmRange"/>.</param>
        /// <returns>Returns true if the range was parsed successfully.</returns>
        /// <exception cref="ArgumentNullException">Thrown if strRange is null.</exception>
        public static bool TryParse(string strRange, out NpmRange range)
        {
            if (strRange == null) throw new ArgumentNullException(nameof(strRange));
            return TryParse(strRange, NpmParseOptions.Default, out range);
        }

        /// <summary>
        /// Tries to parse the range with the given options and returns true if successful.
        /// </summary>
        /// <param name="strRange">The range to parse.</param>
        /// <param name="options">The options to use when parsing.</param>
        /// <param name="range">The parsed range.</param>
        /// <returns>Returns true if the range was parsed successfully.</returns>
        /// <exception cref="ArgumentNullException">Thrown if strRange or options is null.</exception>
        public static bool TryParse(string strRange, NpmParseOptions options, out NpmRange range)
        {
            if (strRange == null) throw new ArgumentNullException(nameof(strRange));
            if (options == null) throw new ArgumentNullException(nameof(options));

            try
            {
                range = Parse(strRange, options);
                return true;
            }
            catch (FormatException)
            {
                range = null;
                return false;
            }
        }

        /// <inheritdoc />
        public bool Contains(SemVersion version)
        {
            bool anySuccess = false;
            
            foreach (NpmComparator[] comps in Ranges)
            {
                // All comparators in range must succeed
                bool failed = false;

                foreach (NpmComparator comp in comps)
                {
                    if (!comp.Includes(version))
                    {
                        failed = true;
                        break;
                    }
                }

                if (!failed)
                {
                    // Success if at least one range includes the version
                    anySuccess = true;
                    break;
                }
            }

            return anySuccess;
        }

        /// <summary>
        /// Returns a string of the included versions in this range.
        /// </summary>
        /// <returns>A string of the included versions in this range.</returns>
        public override string ToString()
        {
            if (cachedStringValue != null)
                return cachedStringValue;

            cachedStringValue = string.Join($" {RangeParser.OrSeparator[0]} ", Ranges.Select(comps => string.Join(" ", comps.Select(comp => comp.ToString()))));
            
            return cachedStringValue;
        }
    }
}
