"""
Copyright (c) 2013 Samuel B. Fries

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
"""

import sublime
import sublime_plugin

"""
    Commands that give the user information about their selections
"""

"""
    Displays a message dialog containing the unicode point for each selected character
"""


class CharValueCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        messages = []

        for region in regions:
            char = view.substr(region)
            messages.append("%s: %s" % (char, hex(ord(char))))

        sublime.message_dialog("Selected characters: \n%s" % ", ".join(messages))

"""
    Displays a message dialog that explains how long length of all selections summed is
"""


class SelectionLengthCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        text = ""
        for region in regions:

            text += self.view.substr(region)

        sublime.message_dialog("Selections are %d characters long." % len(text))

"""
    Displays a message dialog saying how many selections exist.
"""


class CountSelectionCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        sublime.message_dialog("%d selected." % len(regions))
