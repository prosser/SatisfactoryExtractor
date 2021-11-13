namespace Rosser.SatisfactoryExtractor.Validation;

using Rosser.SatisfactoryExtractor.Resources;

/// <summary>Class that uses fluent method chaining to perform one or more validations.</summary>
public class FluentValidator
{
    private readonly List<string> errors = new();
    private readonly bool throwOnError;

    /// <summary>Initializes a new instance of <see cref="FluentValidator" />.</summary>
    /// <param name="throwOnError">If true, an Error is thrown when any validation fails. Otherwise, error messages are appended to the errors property.</param>
    public FluentValidator(bool throwOnError = false)
    {
        this.throwOnError = throwOnError;
    }

    /// <summary>
    /// Gets the list of error messages.
    /// </summary>
    public IReadOnlyList<string> Errors => this.errors;

    /// <summary>
    /// Gets a value indicating whether any errors were encountered.
    /// </summary>
    public bool IsValid => this.errors.Count == 0;

    /// <summary>Validates that all items have a true result on their tests.</summary>
    /// <param name="tests">Tests to perform.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator All(IReadOnlyList<FluentValidatorTest> tests, string? message = null)
    {
        int count = GetPassedCount(tests);
        if (count != tests.Count)
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorAll, string.Join(", ", tests.Select(x => x.Name)));
        }

        return this;
    }

    /// <summary>Validates that at least one of the items has a true result on its test.</summary>
    /// <param name="tests">Tests to perform.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator Any(IReadOnlyList<FluentValidatorTest> tests, string? message = null)
    {
        int count = GetPassedCount(tests);
        if (tests.Any(x => !x.Test()))
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorAny, string.Join(", ", tests.Select(x => x.Name)));
        }

        return this;
    }

    /// <summary>
    /// Validates that a path exists and is a directory.
    /// </summary>
    /// <param name="tests">Tests to perform.</param>
    /// <param name="path">Directory path to validate.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator DirectoryExists(string path, string? message = null)
    {
        if (!Directory.Exists(path))
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorDirectoryExists, path);
        }

        return this;
    }

    /// <summary>
    /// Adds an explicit error message.
    /// </summary>
    /// <param name="message">Error message to add.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator Fail(string message)
    {
        this.errors.Add(message);
        return this;
    }

    /// <summary>
    /// Validates that a path exists and is a file.
    /// </summary>
    /// <param name="path">Path to validate.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator FileExists(string path, string? message = null)
    {
        if (!File.Exists(path))
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorFileExists, path);
        }

        return this;
    }

    /// <summary>
    /// Validates that a path does not exist.
    /// </summary>
    /// <param name="path">Path to validate.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator FsDoesNotExist(string path, string? message = null)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorFsDoesNotExist, path);
        }

        return this;
    }

    /// <summary>
    /// Validates that only one of the tests produces a <c>true</c> result.
    /// </summary>
    /// <param name="tests">Tests to execute.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator Single(IEnumerable<FluentValidatorTest> tests, string? message = null)
    {
        int count = tests.Aggregate(0,
          (count, cur) =>
          {
              if (cur.Test())
              {
                  count++;
              }

              return count;
          });
        if (count > 1)
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorMutuallyExclusive, string.Join(", ", tests.Select(x => x.Name)));
        }

        return this;
    }

    /// <summary>
    /// Validates that none of the tests produce a <c>true</c> result.
    /// </summary>
    /// <param name="tests">Tests to execute.</param>
    /// <param name="message">Error message to use if the validation fails. If null, a default value will be used.</param>
    /// <returns>The <see cref="FluentValidator"/>.</returns>
    public FluentValidator None(IReadOnlyList<FluentValidatorTest> tests, string? message = null)
    {
        int count = GetFailedCount(tests);
        if (count != 0)
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorNone, string.Join(", ", tests.Select(x => x.Name)));
        }

        return this;
    }

    public FluentValidator OneOf<T>(string name, T value, IEnumerable<T> mustBeOneOf, string? message = null)
    {
        int count = mustBeOneOf.Aggregate(0,
          (count, cur) =>
          {
              if (Equals(value, cur))
              {
                  count++;
              }

              return count;
          });

        if (count == 0)
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorOneOf, name, string.Join(", ", mustBeOneOf.Select(v => v?.ToString() ?? "[null]")));
        }

        return this;
    }

    /**
     * Validates that the value exactly equals none of the specified values.
     * @param tests Tests to perform.
     * @param message Error message to use if the validation fails. The default value is
     * "none of $1 can be specified", where $1 is replaced with the names of the values.
     * @returns The FluentValidator
     */
    /**
     * Validates that the path exists and is a directory.
     * @param path Path to test.
     * @param message Error message to use if the validation fails. The default value is
     * "$1 must be an existing directory", where $1 is replaced with path.
     * @returns The FluentValidator
     */
    /**
     * Adds an explicit error to the errors array.
     * @param message Error message to add.
     * @returns The FluentValidator
     */
    /**
     * Validates that the path exists and is a file.
     * @param path Path to test.
     * @param message Error message to use if the validation fails. The default value is
     * "$1 must be an existing file", where $1 is replaced with path.
     * @returns The FluentValidator
     */
    /**
     * Validates that a filesystem entry does not exist for the path.
     * @param path Path to test.
     * @param message Error message to use if the validation fails. The default value is
     * "$1 must not be an existing file or directory", where $1 is replaced with path.
     * @returns the FluentValidator
     */
    /**
     * Validates that only one of the tests has a true result.
     * @param tests Tests to perform.
     * @param message Error message to use if the validation fails. The default value is
     * "$1 are mutually exclusive", where $1 is replaced with the comma-delimited names of the tests.
     * @returns the FluentValidator
     */
    /**
     * Validates that the test passes.
     * @param name Name of the item being validated.
     * @param test Test to perform.
     * @param message Error message to use if the validation fails. The default value is
     * "$1 is required", where $1 is replaced with name.
     * @returns The FluentValidator
     */

    /// <summary>Throws an error if any errors have been encountered.</summary>
    public void ThrowIfErrors(string? helpText = null)
    {
        if (this.errors.Count > 0)
        {
            if (helpText is not null)
            {
                this.errors.Insert(0, helpText);
            }

            throw new FluentValidatorException(string.Join('\n', this.errors));
        }
    }

    public FluentValidator True(string name, Func<bool> test, string? message = null)
    {
        if (!test())
        {
            this.AddError(message ?? ErrorStrings.FluentValidatorRequired, name);
        }

        return this;
    }

    private static int GetFailedCount(IEnumerable<FluentValidatorTest> tests)
    {
        return tests.Aggregate(0, (count, cur) =>
        {
            if (!cur.Test())
            {
                count++;
            }

            return count;
        });
    }

    private static int GetPassedCount(IEnumerable<FluentValidatorTest> tests)
    {
        return tests.Aggregate(0, (count, cur) =>
        {
            if (cur.Test())
            {
                count++;
            }

            return count;
        });
    }

    private void AddError(string error, params string[] replacements)
    {
        string err = replacements.Select((x, i) => (x, i: i + 1))
            .Aggregate(error, (agg, value) =>
        {
            return agg.Replace("$" + value.i, value.x);
        });

        if (this.throwOnError)
        {
            throw new FluentValidatorException(err);
        }

        this.errors.Add(err);
    }
}