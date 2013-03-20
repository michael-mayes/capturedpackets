"""
Copyright (c) 2013 Samuel B. Fries

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
"""

import sublime_plugin

"""
    Commands that manipulate textual data
"""

"""
    Sorts the selected text
"""


class SortTextCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        for region in regions:
            values = view.substr(region).split("\n")
            values.sort()
            view.replace(edit, region, "\n".join(values))


"""
    Splits selected text into lines of a specified maximum length, breaking at words.
"""


class LineLengthCommand(sublime_plugin.TextCommand):

    def receive_input(self, length):
        view = self.view
        sel = view.sel()

        try:
            length = int(length)
        except ValueError:
            view.message_dialog("Please enter a numeric line length; %s is not an integer." % length)
            return

        for selected in sel:

            sub = view.substr(selected)

            sub_words = sub.split()

            lines = [""]

            for word in sub_words:
                if len(lines[-1]) + len(word) > length:
                    lines.append(word)
                else:
                    lines[-1] += " " + word

            view.replace(self.edit, selected, "\n".join(lines))

    def run(self, edit):
        self.edit = edit
        self.view.window().show_input_panel("Max Line Length", "", self.receive_input, None, None)
