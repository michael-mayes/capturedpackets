import sublime, sublime_plugin

class SaveAndCloseFileCommand(sublime_plugin.WindowCommand):
    def run(self):
        self.window.run_command("save")
        self.window.run_command("close_file")