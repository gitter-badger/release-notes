# Contributing

Thank you for considering contributing to this project. You are a wonderful person for stopping by.

Following these guidelines shows the other lovely contributors like you that you respect their time. To learn more about how contributors and maintainers are expected to conduct themselves, please read the [Code of Conduct](CODE_OF_CONDUCT.md).

## HELP!
If you need support, please consider 

Please do not use the issue tracker for support questions.

## 1. Do you have a suggestion or want to report a bug?

### Security Issues
If you find a security vulnerability, do NOT open an issue, email security@somewhatabstract.com instead.

In order to determine whether you are dealing with a security issue, ask yourself these two questions:
* Can I access something that's not mine, or something I shouldn't have access to?
* Can I disable something for other people?

If the answer to either of those two questions are "yes", then you're probably dealing with a security issue. Note that even if you answer "no" to both questions, you may still be dealing with a security issue; if you're unsure, just email us (security@somewhatabstract.com).

### Phew, it's nothing to do with security
If you have found a bug or have a feature suggestion, search the [project issues](https://github.com/somewhatabstract/release-notes/issues?q=something) and see if it has already been raised by another community member. If you find an issue has already been raised by another awesome community member, please consider contributing some additional information to the existing ticket; if there is no existing issue, then [make a new one!](https://github.com/somewhatabstract/release-notes/issues/new)

Please do not use the issue tracker for support questions.

## 2. Do you want to write some code?
If you have raised or found an issue that you think you can help with and no one else appears to be working on it, [then fork the repository](https://help.github.com/articles/fork-a-repo) and create a new branch off `master` with a helpful name, like:
```
git checkout -b 999-add-spline-interpolations
```

### Your First Contribution
If you are looking for something to work on, try the [up for grabs](https://github.com/somewhatabstract/release-notes/labels/up%20for%20grabs) label.

### Project Setup
To get your code up and running, have a look at the [development instructions](README.md). If you struggle to get things working and have some suggestions on how the instructions could be improved, consider raising an issue or submitting a pull request to update the instructions.

### Code Style
- Spaces, not tabs
- Indent 4 spaces
- `camelCase` for variables and JavaScript methods
    - Non-static C# member variables should be prefixed with `_`
- `PascalCase` for namespaces, types, C# methods, and constants
- Naming
    - Avoid non-standard abbreviations. For example, `HTTP` and `JSON` are OK.
    - Variable names should describe their content within the context of its use. For example, `startingCount`, `initialColors`.
    - Method names should describe what they do. For example, `getInitialColors`, or `GetRepositories`

### Tests
Don't forget to write tests for the things you are adding or changing, if it makes sense to do so.

## 3. Do you want to submit your changes?
So you have written an amazing new feature, fixed that heinous bug, or written an astounding tutorial. That is awesome.

Always feel free to ask a fellow contributor for help; everyone is a beginner at first!

Before submitting a pull request, you need to make sure that your changes:
1. Pass all the tests
1. Work as intended
1. Are scoped to a single purpose as much as possible (do not implement multiple issues in the same pull request)
1. Follow the project coding and writing styles

If everything looks good, [make a pull request!](https://help.github.com/articles/creating-a-pull-request)

>The pull request template will indicate the information you should include as well as remind you of some of the more important things you have read here, so don't worry if you forget all this.

Fill out the pull request and submit it. This will trigger the automated test suite to kick in and check that the code builds and passes the tests. Assuming everything is fine with, a maintainer will review your contribution and provide feedback as appropriate.

If a maintainer asks you to "rebase" your PR, they're saying that a lot of code has changed, and that you need to update your branch so it's easier to merge. Rebasing can sometimes be tricky, so if you are not comfortable going it alone, ask for help :simple_smile:

If the maintainer feels that changes are needed, they will provide appropriate commentary directing what those changes should be. Otherwise, if the maintainer is satisfied, the pull request will be merged.

# Finally
Have fun. It is great to contribute to the Open Source community and it is great to receive contributions. By taking part in a project, you can find new friends, make new connections, and learn new things. Be open, be willing, and be amazing! Thank you!