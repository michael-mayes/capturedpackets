"""
Copyright (c) 2013 Samuel B. Fries

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
"""

import sublime_plugin
from util import *


"""
    Plugins that deal with numeric data
"""

"""
    Converts a group of selected integers into a sequence (from 0 to N, where N is the number of selections)
"""


class IntegerSequenceCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        ind = 0
        for region in regions:
            val = string_as_integer(self.view.substr(region))

            if val is not None:

                self.view.replace(edit, region, str(ind))

                ind += 1

"""
    Converts selected hex numbers into decimal equivalents
"""


class HexToDecimalCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        for region in regions:
            text = self.view.substr(region)

            try:
                int_val = int(text, base=16)
                self.view.replace(edit, region, str(int_val))
            except ValueError:
                pass


"""
    Converts selected decimal numbers into hexadecimal equivalents
"""


class DecimalToHexCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        for region in regions:

            text = self.view.substr(region)

            num = string_as_integer(text)

            if num is not None:
                text = num_to_hex(num)

            self.view.replace(edit, region, text)

"""
    Increments the selected numbers
"""


class IncrementSelectionCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        #For each positive integer in the selection, increment it by 1
        for region in regions:
            text = self.view.substr(region)

            num = string_as_integer(text)

            if num is not None:
                num += 1
                text = str(num)

            self.view.replace(edit, region, text)

"""
    Decrements the selected numbers
"""


class DecrementSelectionCommand(sublime_plugin.TextCommand):
    def run(self, edit):
        view = self.view
        regions = view.sel()

        #For each positive integer in the selection, increment it by 1
        for region in regions:
            text = self.view.substr(region)
            num = string_as_integer(text)

            if num is not None:
                num -= 1
                text = str(num)

            self.view.replace(edit, region, text)
