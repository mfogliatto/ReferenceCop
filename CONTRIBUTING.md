# Contributing to ReferenceCop

Thanks for checking out the project! We're just getting started, so any help will be appreciated. Here's a quick guide on how you can contribute:

## Got a bug or idea?

- Found a bug? Open an issue to describe what's wrong.
- Have an idea? Open an issue to suggest new features.

## Want to code?

1. Fork the repo
2. Create a new branch (`git checkout -b awesome-feature`)
3. Make your changes
4. Commit (`git commit -am 'Add awesome feature'`)
5. Push to your branch (`git push origin awesome-feature`)
6. Open a Pull Request

## Some quick tips

- Keep your code clean and readable
- Add comments if things get tricky
- Test your changes before submitting.
- Update docs if needed

## Debugging

To attach a debugger during development:

1. **For MSBuild task debugging:**
   - Add `<LaunchDebugger>MSBuild</LaunchDebugger>` to your project file
   - Build the project - the debugger will launch automatically

2. **For Roslyn analyzer debugging:**
   - Add `<LaunchDebugger>Roslyn</LaunchDebugger>` to your project file
   - Add `<CompilerVisibleProperty Include="LaunchDebugger" />` to make the property visible to the analyzer
   - Build the project - the debugger will launch automatically

See examples in the playground project files for reference.

## End-to-End Testing

Use the `playground` directory for end-to-end validation of your changes. See the [playground README](playground/README.md) for detailed instructions on how to test ReferenceCop changes in a real project environment.

## Need help?

No worries! If you're stuck or have questions, just open an issue or create a discussion straight on GitHub :)

Thanks for helping out! 🎉