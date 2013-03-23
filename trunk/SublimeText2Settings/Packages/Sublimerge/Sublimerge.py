 # Copyright (c) 2012 Borys Forytarz <borys.forytarz@gmail.com>
 #
 # Permission is hereby granted, free of charge, to any person
 # obtaining a copy of this software and associated documentation files
 # (the "Software"), to deal in the Software without restriction,
 # including without limitation the rights to use, copy, modify,
 # merge, publish, distribute, sublicense, and/or sell copies of the
 # Software, and to permit persons to whom the Software is furnished
 # to do so, subject to the following conditions:
 #
 # The above copyright notice and this permission notice shall be
 # included in all copies or substantial portions of the Software.
 #
 # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 # EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 # MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 # NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
 # BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 # ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 # CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 # SOFTWARE.
 #
 # https://github.com/borysf/Sublimerge


import sublime
import sublime_plugin
import difflib
import re
import os
import subprocess
import threading
from xml.dom import minidom

diffView = None

settings = sublime.load_settings('Sublimerge.sublime-settings')


class SublimergeSettings():
    s = {
        'same_syntax_only': True,
        'intelligent_files_sort': True,
        'hide_side_bar': True,
        'compact_files_list': True,
        'diff_region_expander_text': '?',
        'diff_region_scope': 'selection',
        'diff_region_added_scope': 'markup.inserted',
        'diff_region_removed_scope': 'markup.deleted',
        'diff_region_gutter_icon': 'none',
        'diff_region_change_scope': 'markup.changed',
        'selected_diff_region_scope': 'selection',
        'selected_diff_region_gutter_icon': 'bookmark',
        'ignore_whitespace': False,
        'ignore_crlf': True,
        'vcs_support': True,
        'git_executable_path': 'git',
        'git_log_args': '',
        'git_show_args': '',
        'svn_executable_path': 'svn',
        'svn_log_args': '',
        'svn_cat_args': ''
    }

    def load(self):
        for name in self.s:
            self.s[name] = settings.get(name, self.s[name])

    def get(self, name):
        return self.s[name]

S = SublimergeSettings()
S.load()
settings.add_on_change('reload', lambda: S.load())


def isSaved(view):
    if view.is_dirty():
        sublime.error_message('File `' + os.path.split(view.file_name())[1] + '` must be saved in order to compare')
        return False

    return True


def lookForVcs(path):
    if not S.get('vcs_support'):
        return False

    if os.path.isdir(path + '/.svn'):
        return 'svn'
    elif os.path.isdir(path + '/.git'):
        return 'git'
    else:
        sp = os.path.split(path)
        if sp[0] != path and sp[0] != '':
            return lookForVcs(sp[0])


def executeShellCmd(exe, cwd):
    print "Cmd: %s" % (exe)
    print "Dir: %s" % (cwd)

    p = subprocess.Popen(exe, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, cwd=cwd, shell=True)

    while(True):
        retcode = p.poll()
        line = re.sub('(^\s+$)|(\s+$)', '', p.stdout.readline())

        if line != '':
            yield line

        if retcode is not None:
            break


class SublimergeDiffer():

    def process(self, line0, line1, line2):
        if line0 == None:
            return

        change = line0[0]
        line0 = line0[2:len(line0)]

        part = None

        if change == '+':
            part = {'+': line0, '-': '', 'change': '+', 'intraline': '', 'intralines': {'+': [], '-': []}}

        elif change == '-':
            part = {'-': line0, '+': '', 'change': '-', 'intraline': '', 'intralines': {'+': [], '-': []}}

        elif change == ' ':
            part = line0

        elif change == '?':
            return

        if isinstance(part, str) and isinstance(self.data[self.lastIdx], str):
            self.data[self.lastIdx] += part
        else:
            if isinstance(part, dict):
                if line1 and line1[0] == '?':
                    part['intraline'] = change

                if self.lastIdx >= 0:
                    last = self.data[self.lastIdx]
                else:
                    last = None

                if isinstance(last, dict):
                    skip = False

                    im_p = last['intraline'] == '-' and part['change'] == '+'
                    im_ip = last['intraline'] == '-' and part['intraline'] == '+'
                    m_ip = last['change'] == '-' and part['intraline'] == '+'

                    if im_p or im_ip or m_ip:
                        self.data[self.lastIdx]['+'] += part['+']
                        self.data[self.lastIdx]['-'] += part['-']
                        self.data[self.lastIdx]['intraline'] = '!'
                        skip = True
                    elif part['intraline'] == '' and last['intraline'] == '':
                        nextIntraline = None
                        if line2 and line2[0] == '?':
                            nextIntraline = line1[0]

                        if nextIntraline == '+' and part['change'] == '-':
                            self.data.append(part)
                            self.lastIdx += 1
                            skip = True
                        else:
                            self.data[self.lastIdx]['+'] += part['+']
                            self.data[self.lastIdx]['-'] += part['-']
                            skip = True

                    if not skip:
                        self.data.append(part)
                        self.lastIdx += 1
                else:
                    self.data.append(part)
                    self.lastIdx += 1
            else:
                self.data.append(part)
                self.lastIdx += 1

    def difference(self, text1, text2):
        self.data = []
        self.lastIdx = -1
        gen = difflib.Differ().compare(text1.splitlines(1), text2.splitlines(1))

        line0 = None
        line1 = None
        line2 = None

        try:
            line0 = gen.next()
            line1 = gen.next()
        except:
            pass

        inFor = False

        for line2 in gen:
            self.process(line0, line1, line2)
            line0 = line1
            line1 = line2
            inFor = True

        self.process(line0, line1, None)

        if not inFor:
            self.process(line1, line2, None)

        self.process(line2, None, None)

        return self.data


class SublimergeScrollSync():
    left = None
    right = None
    scrollingView = None
    viewToSync = None
    lastPosLeft = None
    lastPosRight = None
    isRunning = False
    last = None
    targetPos = None

    def __init__(self, left, right):
        self.left = left
        self.right = right
        self.sync()

    def sync(self):
        beginLeft = self.left.viewport_position()
        beginRight = self.right.viewport_position()

        if not self.isRunning:
            if beginLeft[0] != beginRight[0] or beginLeft[1] != beginRight[1]:
                if self.lastPosLeft == None or (self.lastPosLeft[0] != beginLeft[0] or self.lastPosLeft[1] != beginLeft[1]):
                    self.isRunning = True
                    self.scrollingView = self.left
                    self.viewToSync = self.right

                elif self.lastPosRight == None or (self.lastPosRight[0] != beginRight[0] or self.lastPosRight[1] != beginRight[1]):
                    self.isRunning = True
                    self.scrollingView = self.right
                    self.viewToSync = self.left

        else:
            pos = self.scrollingView.viewport_position()

            if self.targetPos == None and self.last != None and pos[0] == self.last[0] and pos[1] == self.last[1]:
                ve = self.viewToSync.viewport_extent()
                le = self.viewToSync.layout_extent()

                self.targetPos = (max(0, min(pos[0], le[0] - ve[0])), max(0, min(pos[1], le[1] - ve[1])))
                self.viewToSync.set_viewport_position(self.targetPos)

            elif self.targetPos != None:
                poss = self.viewToSync.viewport_position()

                if poss[0] == self.targetPos[0] and poss[1] == self.targetPos[1]:
                    self.isRunning = False
                    self.targetPos = None
                    self.scrollingView = None
                    self.viewToSync = None

            self.last = pos

        self.lastPosRight = beginRight
        self.lastPosLeft = beginLeft

        if self.left.window() != None and self.right.window() != None:
            sublime.set_timeout(self.sync, 100)


class SublimergeView():
    left = None
    right = None
    window = None
    currentDiff = -1
    regions = []
    currentRegion = None
    scrollSyncRunning = False
    lastLeftPos = None
    lastRightPos = None
    diff = None
    createdPositions = False
    lastSel = {'regionLeft': None, 'regionRight': None}

    def __init__(self, window, left, right, diff, leftTmp=False, rightTmp=False):
        window.run_command('new_window')
        self.window = sublime.active_window()
        self.diff = diff
        self.leftTmp = leftTmp
        self.rightTmp = rightTmp

        if (S.get('hide_side_bar')):
            self.window.run_command('toggle_side_bar')

        self.window.set_layout({
            "cols": [0.0, 0.5, 1.0],
            "rows": [0.0, 1.0],
            "cells": [[0, 0, 1, 1], [1, 0, 2, 1]]
        })

        if isinstance(left, unicode):
            self.left = self.window.open_file(left)
        else:
            self.left = self.window.open_file(left.file_name())

        if isinstance(right, unicode):
            self.right = self.window.open_file(right)
        else:
            self.right = self.window.open_file(right.file_name())

        if not leftTmp and rightTmp:
            self.right.set_syntax_file(self.left.settings().get('syntax'))

        self.left.set_scratch(True)
        self.right.set_scratch(True)

        if self.leftTmp and isinstance(left, unicode):
            os.remove(left)
        if self.rightTmp and isinstance(right, unicode):
            os.remove(right)

    def enlargeCorrespondingPart(self, part1, part2):
        linesPlus = part1.splitlines()
        linesMinus = part2.splitlines()

        diffLines = len(linesPlus) - len(linesMinus)

        if diffLines < 0:  # linesPlus < linesMinus
            for i in range(-diffLines):
                linesPlus.append(S.get('diff_region_expander_text'))

        elif diffLines > 0:  # linesPlus > linesMinus
            for i in range(diffLines):
                linesMinus.append(S.get('diff_region_expander_text'))

        result = []

        result.append("\n".join(linesPlus) + "\n")
        result.append("\n".join(linesMinus) + "\n")

        return result

    def loadDiff(self):
        self.window.set_view_index(self.right, 1, 0)
        sublime.set_timeout(lambda: self.insertDiffContents(self.diff), 5)

    def insertDiffContents(self, diff):
        left = self.left
        right = self.right

        edit = left.begin_edit()
        left.erase(edit, sublime.Region(0, left.size()))
        left.end_edit(edit)

        edit = right.begin_edit()
        right.erase(edit, sublime.Region(0, right.size()))
        right.end_edit(edit)

        regions = []
        i = 0

        for part in diff:
            if not isinstance(part, dict):
                edit = left.begin_edit()
                left.insert(edit, left.size(), part)
                left.end_edit(edit)

                edit = right.begin_edit()
                right.insert(edit, right.size(), part)
                right.end_edit(edit)
            else:
                ignore = False

                if S.get('ignore_whitespace'):
                    trimRe = '(^\s+)|(\s+$)'
                    if re.sub(trimRe, '', part['+']) == re.sub(trimRe, '', part['-']):
                        ignore = True

                if ignore:
                    edit = left.begin_edit()
                    left.insert(edit, left.size(), part['-'])
                    left.end_edit(edit)

                    edit = right.begin_edit()
                    right.insert(edit, right.size(), part['+'])
                    right.end_edit(edit)
                    continue

                pair = {
                    'regionLeft': None,
                    'regionRight': None,
                    'name': 'diff' + str(i),
                    'mergeLeft': part['+'][:],
                    'mergeRight': part['-'][:],
                    'intralines': {'left': [], 'right': []}
                }

                i += 1

                edit = left.begin_edit()
                leftStart = left.size()

                if part['+'] != '' and part['-'] != '' and part['intraline'] != '':
                    inlines = list(difflib.Differ().compare(part['-'].splitlines(1), part['+'].splitlines(1)))
                    begins = {'+': 0, '-': 0}
                    lastLen = 0
                    lastChange = None

                    for inline in inlines:
                        change = inline[0:1]
                        inline = inline[2:len(inline)]
                        inlineLen = len(inline)

                        if change != '?':
                            begins[change] += inlineLen
                            lastLen = inlineLen
                            lastChange = change
                        else:
                            for m in re.finditer('([+-^]+)', inline):
                                sign = m.group(0)[0:1]

                                if sign == '^':
                                    sign = lastChange

                                start = begins[sign] - lastLen + m.start()
                                end = begins[sign] - lastLen + m.end()

                                part['intralines'][sign].append([start, end])

                enlarged = self.enlargeCorrespondingPart(part['+'], part['-'])

                left.insert(edit, leftStart, enlarged[1])
                left.end_edit(edit)

                edit = right.begin_edit()
                rightStart = right.size()
                right.insert(edit, rightStart, enlarged[0])
                right.end_edit(edit)

                pair['regionLeft'] = sublime.Region(leftStart, leftStart + len(left.substr(sublime.Region(leftStart, left.size()))))
                pair['regionRight'] = sublime.Region(rightStart, rightStart + len(right.substr(sublime.Region(rightStart, right.size()))))

                if pair['regionLeft'] != None and pair['regionRight'] != None:
                    for position in part['intralines']['-']:
                        change = sublime.Region(leftStart + position[0], leftStart + position[1])
                        pair['intralines']['left'].append(change)

                    for position in part['intralines']['+']:
                        change = sublime.Region(rightStart + position[0], rightStart + position[1])
                        pair['intralines']['right'].append(change)

                    regions.append(pair)

        for pair in regions:
            self.createDiffRegion(pair)

        self.createdPositions = True

        self.regions = regions
        sublime.set_timeout(lambda: self.selectDiff(0), 100)  # for some reason this fixes the problem to scroll both views to proper position after loading diff

        self.left.set_read_only(True)
        self.right.set_read_only(True)
        SublimergeScrollSync(self.left, self.right)

    def createDiffRegion(self, region):
        rightScope = leftScope = S.get('diff_region_scope')

        if region['mergeLeft'] == '':
            rightScope = S.get('diff_region_removed_scope')
            leftScope = S.get('diff_region_added_scope')
        elif region['mergeRight'] == '':
            leftScope = S.get('diff_region_removed_scope')
            rightScope = S.get('diff_region_added_scope')

        if not self.createdPositions:
            self.left.add_regions('intralines' + region['name'], region['intralines']['left'], S.get('diff_region_change_scope'))
            self.right.add_regions('intralines' + region['name'], region['intralines']['right'], S.get('diff_region_change_scope'))

        self.left.add_regions(region['name'], [region['regionLeft']], leftScope, S.get('diff_region_gutter_icon'), sublime.DRAW_OUTLINED)
        self.right.add_regions(region['name'], [region['regionRight']], rightScope, S.get('diff_region_gutter_icon'), sublime.DRAW_OUTLINED)

    def createSelectedRegion(self, region):
        self.left.add_regions(region['name'], [region['regionLeft']], S.get('selected_diff_region_scope'), S.get('selected_diff_region_gutter_icon'))
        self.right.add_regions(region['name'], [region['regionRight']], S.get('selected_diff_region_scope'), S.get('selected_diff_region_gutter_icon'))

    def selectDiff(self, diffIndex):
        if diffIndex >= 0 and diffIndex < len(self.regions):
            self.left.sel().clear()
            self.left.sel().add(sublime.Region(0, 0))
            self.right.sel().clear()
            self.right.sel().add(sublime.Region(0, 0))

            if self.currentRegion != None:
                self.createDiffRegion(self.currentRegion)

            self.currentRegion = self.regions[diffIndex]
            self.createSelectedRegion(self.currentRegion)

            self.currentDiff = diffIndex

            self.left.show_at_center(sublime.Region(self.currentRegion['regionLeft'].begin(), self.currentRegion['regionLeft'].begin()))
            if not S.get('ignore_whitespace'):  # @todo: temporary fix for loosing view sync while ignore_whitespace is true
                self.right.show_at_center(sublime.Region(self.currentRegion['regionRight'].begin(), self.currentRegion['regionRight'].begin()))

    def selectDiffUnderSelection(self, selection, side):
        if self.createdPositions:
            if selection[0].begin() == 0 and selection[0].end() == 0:  # this fixes strange behavior with regions
                return

            for i in range(len(self.regions)):
                if self.regions[i][side].contains(selection[0]):
                    self.selectDiff(i)
                    break

    def checkForClick(self, view):
        side = None

        if view.id() == self.left.id():
            side = 'regionLeft'
        elif view.id() == self.right.id():
            side = 'regionRight'

        if side != None:
            sel = [r for r in view.sel()]

            if self.lastSel[side]:
                if sel == self.lastSel[side]:  # previous selection equals current so it means this was a mouse click!
                    self.selectDiffUnderSelection(view.sel(), side)

            self.lastSel[side] = sel

    def goUp(self):
        self.selectDiff(self.currentDiff - 1)

    def goDown(self):
        self.selectDiff(self.currentDiff + 1)

    def merge(self, direction, mergeAll):
        if (self.rightTmp and direction == '>>') or (self.leftTmp and direction == '<<'):
            return

        if mergeAll:
            while len(self.regions) > 0:
                self.merge(direction, False)
            return

        if (self.currentRegion != None):
            lenLeft = self.left.size()
            lenRight = self.right.size()
            if direction == '<<':
                source = self.right
                target = self.left
                sourceRegion = self.currentRegion['regionRight']
                targetRegion = self.currentRegion['regionLeft']
                contents = self.currentRegion['mergeLeft']

            elif direction == '>>':
                source = self.left
                target = self.right
                sourceRegion = self.currentRegion['regionLeft']
                targetRegion = self.currentRegion['regionRight']
                contents = self.currentRegion['mergeRight']

            target.set_scratch(True)

            target.set_read_only(False)
            source.set_read_only(False)

            edit = target.begin_edit()
            target.replace(edit, targetRegion, contents)
            target.end_edit(edit)

            edit = source.begin_edit()
            source.replace(edit, sourceRegion, contents)
            source.end_edit(edit)

            diffLenLeft = self.left.size() - lenLeft
            diffLenRight = self.right.size() - lenRight

            source.erase_regions(self.currentRegion['name'])
            target.erase_regions(self.currentRegion['name'])
            source.erase_regions('intralines' + self.currentRegion['name'])
            target.erase_regions('intralines' + self.currentRegion['name'])

            target.set_scratch(False)

            del self.regions[self.currentDiff]

            for i in range(self.currentDiff, len(self.regions)):
                self.regions[i]['regionLeft'] = self.moveRegionBy(self.regions[i]['regionLeft'], diffLenLeft)
                self.regions[i]['regionRight'] = self.moveRegionBy(self.regions[i]['regionRight'], diffLenRight)

                # for j in range(self.currentDiff, len(self.regions[i]['intralines']['left'])):
                #     self.regions[i]['intralines']['left'][j] = self.moveRegionBy(self.regions[i]['intralines']['left'][j], diffLenLeft)

                # for j in range(self.currentDiff, len(self.regions[i]['intralines']['right'])):
                #     self.regions[i]['intralines']['right'][j] = self.moveRegionBy(self.regions[i]['intralines']['right'][j], diffLenRight)

                if i != self.currentDiff:
                    self.createDiffRegion(self.regions[i])

            target.set_read_only(True)
            source.set_read_only(True)

            if self.currentDiff > len(self.regions) - 1:
                self.currentDiff = len(self.regions) - 1

            self.currentRegion = None

            if self.currentDiff >= 0:
                self.selectDiff(self.currentDiff)
            else:
                self.currentDiff = -1

            self.window.focus_view(target)

    def moveRegionBy(self, region, by):
        return sublime.Region(region.begin() + by, region.end() + by)

    def abandonUnmergedDiffs(self, side):
        if side == 'left':
            view = self.left
            regionKey = 'regionLeft'
            contentKey = 'mergeRight'
        elif side == 'right':
            view = self.right
            regionKey = 'regionRight'
            contentKey = 'mergeLeft'

        view.set_read_only(False)
        edit = view.begin_edit()

        for i in range(len(self.regions)):
            sizeBefore = view.size()
            view.replace(edit, self.regions[i][regionKey], self.regions[i][contentKey])
            sizeDiff = view.size() - sizeBefore

            if sizeDiff != 0:
                for j in range(i, len(self.regions)):
                    self.regions[j][regionKey] = sublime.Region(self.regions[j][regionKey].begin() + sizeDiff, self.regions[j][regionKey].end() + sizeDiff)

        view.end_edit(edit)
        view.set_read_only(True)


class ThreadProgress():
    def __init__(self, thread, message):
        self.th = thread
        self.msg = message
        self.add = 1
        self.size = 8
        self.speed = 100
        sublime.set_timeout(lambda: self.run(0), self.speed)

    def run(self, i):
        if not self.th.is_alive():
            if hasattr(self.th, 'result') and not self.th.result:
                sublime.status_message('')
            return

        before = i % self.size
        after = (self.size - 1) - before

        sublime.status_message('%s [%s=%s]' % (self.msg, ' ' * before, ' ' * after))

        if not after:
            self.add = -1
        if not before:
            self.add = 1

        i += self.add

        sublime.set_timeout(lambda: self.run(i), self.speed)


class SublimergeDiffThread(threading.Thread):
    def __init__(self, window, left, right, leftTmp=False, rightTmp=False):
        self.window = window
        self.left = left
        self.right = right
        self.leftTmp = leftTmp
        self.rightTmp = rightTmp

        #self.text1 = self.left.substr(sublime.Region(0, self.left.size()))

        if isinstance(self.left, unicode):
            self.text1 = open(self.left, 'rb').read().decode('utf-8', 'replace')
        else:
            self.text1 = self.left.substr(sublime.Region(0, self.left.size()))

        if isinstance(self.right, unicode):
            self.text2 = open(self.right, 'rb').read().decode('utf-8', 'replace')
        else:
            self.text2 = self.right.substr(sublime.Region(0, self.right.size()))

        threading.Thread.__init__(self)

    def run(self):
        ThreadProgress(self, 'Computing differences')

        global diffView

        differs = False

        if S.get('ignore_crlf'):
            self.text1 = re.sub('\r\n', '\n', self.text1)
            self.text2 = re.sub('\r\n', '\n', self.text2)

            self.text1 = re.sub('\r', '\n', self.text1)
            self.text2 = re.sub('\r', '\n', self.text2)

        if S.get('ignore_whitespace'):
            regexp = re.compile('(^\s+)|(\s+$)', re.MULTILINE)
            if re.sub(regexp, '', self.text1) != re.sub(regexp, '', self.text2):
                differs = True
        elif self.text1 != self.text2:
            differs = True

        if not differs:
            sublime.error_message('There is no difference between files')
            if self.leftTmp and isinstance(self.left, unicode):
                os.remove(self.left)
            if self.rightTmp and isinstance(self.right, unicode):
                os.remove(self.right)
            return

        diff = SublimergeDiffer().difference(self.text1, self.text2)

        def inner():
            global diffView
            diffView = SublimergeView(self.window, self.left, self.right, diff, self.leftTmp, self.rightTmp)

        sublime.set_timeout(inner, 100)


class SublimergeHistoryThread(threading.Thread):
    def __init__(self, filename, vcs, sublimerge):
        self.filename = filename
        self.sublimerge = sublimerge
        self.vcs = vcs
        threading.Thread.__init__(self)

    def run(self):
        ThreadProgress(self, 'Fetching commits history')
        if self.vcs == 'git':
            self.fetchFromGit()
        elif self.vcs == 'svn':
            self.fetchFromSvn()

    def fetchFromSvn(self):
        sp = os.path.split(self.filename)
        cmd = '%s log "%s" --xml %s' % (S.get('svn_executable_path'), sp[1], S.get('svn_log_args'))
        xml = ''

        for line in executeShellCmd(cmd, sp[0]):
            xml += line

        if xml != '' and re.match('^<\?xml', xml):
            try:
                dom = minidom.parseString(xml)
                commitStack = []
                for entry in dom.getElementsByTagName('logentry'):
                    author = entry.getElementsByTagName('author')[0].childNodes[0].nodeValue
                    date = entry.getElementsByTagName('date')[0].childNodes[0].nodeValue
                    msgs = entry.getElementsByTagName('msg')

                    if len(msgs) > 0 and len(msgs[0].childNodes) > 0:
                        msg = msgs[0].childNodes[0].nodeValue.splitlines()
                    else:
                        msg = []

                    commitStack.append({'commit': entry.getAttribute('revision'), 'author': author, 'date': date, 'msg': msg})

                self.displayQuickPanel(commitStack, self.sublimerge.onListSelectSvn)
            except:
                sublime.error_message('Unable to parse XML')

        elif xml != '':
            sublime.error_message(xml.decode('utf-8', 'replace'))

        else:
            sublime.error_message('Empty svn log output')

    def fetchFromGit(self):
        commitStack = []
        outputStack = []

        def addCommitStack(line):
            match = re.match('^commit\s+([a-zA-Z0-9]+)$', line)

            if match:
                commitStack.append({'commit': match.group(1), 'date': '', 'author': '', 'msg': []})
            elif len(commitStack) > 0:
                match = re.match('^Author:\s+(.+)$', line)
                if match:
                    commitStack[len(commitStack) - 1]['author'] = match.group(1).decode('utf-8', 'replace')
                else:
                    match = re.match('^Date:\s+(.+)$', line)
                    if match:
                        commitStack[len(commitStack) - 1]['date'] = match.group(1).decode('utf-8', 'replace')
                    else:
                        commitStack[len(commitStack) - 1]['msg'].append(line.decode('utf-8', 'replace'))
            else:
                outputStack.append(line.decode('utf-8', 'replace'))

        sp = os.path.split(self.filename)

        cmd = '%s log %s -- "%s"' % (S.get('git_executable_path'), S.get('git_log_args'), sp[1])

        for line in executeShellCmd(cmd, sp[0]):
            addCommitStack(line)

        if len(outputStack) > 0:
            sublime.error_message("\n".join(outputStack))
            return

        self.displayQuickPanel(commitStack, self.sublimerge.onListSelectGit)

    def displayQuickPanel(self, commitStack, callback):
        def inner():
            self.sublimerge.displayQuickPanel(commitStack, callback)

        if len(commitStack) == 0:
            sublime.error_message("No history for file\n%s\nIs it versioned?" % (self.filename))
        else:
            sublime.set_timeout(inner, 100)


class SublimergeCommand(sublime_plugin.WindowCommand):
    viewsPaths = []
    viewsList = []
    itemsList = []
    commits = []
    window = None
    view = None
    
    def is_enabled(self):
        view = sublime.active_window().active_view();
        if diffView and  diffView.left and diffView.right and view and (view.id() == diffView.left.id() or view.id() == diffView.right.id()):
            return False

        return True

    def getComparableFiles(self):
        self.viewsList = []
        self.viewsPaths = []
        active = self.window.active_view()

        allViews = self.window.views()
        ratios = []
        if S.get('intelligent_files_sort'):
            original = os.path.split(active.file_name())

        for view in allViews:
            if view.file_name() != None and view.file_name() != active.file_name() and (not S.get('same_syntax_only') or view.settings().get('syntax') == active.settings().get('syntax')):
                f = view.file_name()

                ratio = 0

                if S.get('intelligent_files_sort'):
                    ratio = difflib.SequenceMatcher(None, original[1], os.path.split(f)[1]).ratio()

                ratios.append({'ratio': ratio, 'file': f, 'dirname': ''})

        ratiosLength = len(ratios)

        if ratiosLength > 0:
            ratios.sort(self.sortFiles)

            if S.get('compact_files_list'):
                for i in range(ratiosLength):
                    for j in range(ratiosLength):
                        if i != j:
                            sp1 = os.path.split(ratios[i]['file'])
                            sp2 = os.path.split(ratios[j]['file'])

                            if sp1[1] == sp2[1]:
                                ratios[i]['dirname'] = self.getFirstDifferentDir(sp1[0], sp2[0])
                                ratios[j]['dirname'] = self.getFirstDifferentDir(sp2[0], sp1[0])

            for f in ratios:
                self.viewsPaths.append(f['file'])
                self.viewsList.append(self.prepareListItem(f['file'], f['dirname']))

            self.window.show_quick_panel(self.viewsList, self.onListSelect)
        else:
            if S.get('same_syntax_only'):
                syntax = re.match('(.+)\.tmLanguage$', os.path.split(active.settings().get('syntax'))[1])
                if syntax != None:
                    sublime.error_message('There are no other open ' + syntax.group(1) + ' files to compare')
                    return

            sublime.error_message('There are no other open files to compare')

    def run(self):
        self.window = sublime.active_window()
        self.active = self.window.active_view()

        if not self.active or not isSaved(self.active):
            return

        sp = os.path.split(self.active.file_name())
        vcs = lookForVcs(sp[0])

        def onMenuSelect(index):
            if index == 0:
                self.getComparableFiles()
            elif index == 1:
                th = SublimergeHistoryThread(self.active.file_name(), vcs, self)
                th.start()

        items = ['Compare to other file...']

        if vcs:
            items.append('Compare to revision...')

        if len(items) > 1:
            self.window.show_quick_panel(items, onMenuSelect)
        else:
            self.getComparableFiles()

    def onListSelectGit(self, index):
        sp = os.path.split(self.active.file_name())

        if index >= 0:
            outfile = '%s/%s@%s' % (sp[0], sp[1], self.commits[index][0:10])
            cmd = '%s show %s %s:"./%s" > "%s"' % (S.get('git_executable_path'), S.get('git_show_args'), self.commits[index], sp[1], outfile)

            for line in executeShellCmd(cmd, sp[0]):
                print line

            th = SublimergeDiffThread(self.window, self.active, outfile, rightTmp=True)
            th.start()

        return False

    def onListSelectSvn(self, index):
        if index >= 0:
            sp = os.path.split(self.active.file_name())

            outfile = '%s/%s@%s' % (sp[0], sp[1], self.commits[index])

            cmd = '%s cat "%s"@%s %s > "%s"' % (S.get('svn_executable_path'), sp[1], self.commits[index], S.get('svn_cat_args'), outfile)

            for line in executeShellCmd(cmd, sp[0]):
                print line

            th = SublimergeDiffThread(self.window, self.active, outfile, rightTmp=True)
            th.start()

    def displayQuickPanel(self, commitStack, callback):
        sublime.status_message('')

        self.itemsList = []
        self.commits = []
        for item in commitStack:
            self.commits.append(item['commit'])
            itm = [item['commit'][0:10] + ' @ ' + item['date'], item['author']]
            if len(item['msg']) > 0:
                line = re.sub('(^\s+)|(\s+$)', '', item['msg'][0])
                itm.append(line)

            self.itemsList.append(itm)

        self.window.show_quick_panel(self.itemsList, callback)

    def prepareListItem(self, name, dirname):
        if S.get('compact_files_list'):
            sp = os.path.split(name)

            if dirname != None and dirname != '':
                dirname = ' / ' + dirname
            else:
                dirname = ''

            if (len(sp[0]) > 56):
                p1 = sp[0][0:20]
                p2 = sp[0][-36:]
                return [sp[1] + dirname, p1 + '...' + p2]
            else:
                return [sp[1] + dirname, sp[0]]
        else:
            return name

    def getFirstDifferentDir(self, a, b):
        a1 = re.split('[/\\\]', a)
        a2 = re.split('[/\\\]', b)

        len2 = len(a2) - 1

        for i in range(len(a1)):
            if i > len2 or a1[i] != a2[i]:
                return a1[i]

    def sortFiles(self, a, b):
        d = b['ratio'] - a['ratio']

        if d == 0:
            return 0
        if d < 0:
            return -1
        if d > 0:
            return 1

    def onListSelect(self, itemIndex):
        if itemIndex > -1:
            allViews = self.window.views()
            compareTo = None
            for view in allViews:
                if (view.file_name() == self.viewsPaths[itemIndex]):
                    compareTo = view
                    break

            if compareTo != None:
                global diffView

                if isSaved(compareTo):
                    th = SublimergeDiffThread(self.window, self.window.active_view(), compareTo)
                    th.start()


class SublimergeGoUpCommand(sublime_plugin.WindowCommand):
    def run(self):
        if diffView != None:
            diffView.goUp()


class SublimergeGoDownCommand(sublime_plugin.WindowCommand):
    def run(self):
        if diffView != None:
            diffView.goDown()


class SublimergeMergeLeftCommand(sublime_plugin.WindowCommand):
    def run(self, mergeAll=False):
        if diffView != None:
            diffView.merge('<<', mergeAll)


class SublimergeMergeRightCommand(sublime_plugin.WindowCommand):
    def run(self, mergeAll=False):
        if diffView != None:
            diffView.merge('>>', mergeAll)


class SublimergeDiffSelectedFiles(sublime_plugin.WindowCommand):
    def run(self, files):
        allViews = self.window.views()
        for view in allViews:
            if (view.file_name() == files[0] or view.file_name() == files[1]) and not isSaved(view):
                return

        th = SublimergeDiffThread(self.window, files[0], files[1])
        th.start()

    def is_enabled(self, files):
        return len(files) == 2


class SublimergeFromSidebar(sublime_plugin.WindowCommand):
    def is_enabled(self, files):
        return len(files) == 1

    def run(self, files):
        sublime.active_window().open_file(files[0], sublime.TRANSIENT)
        sublime.active_window().run_command('sublimerge')


class SublimergeListener(sublime_plugin.EventListener):
    left = None
    right = None

    def on_load(self, view):
        global diffView

        if diffView != None:
            if view.id() == diffView.left.id():
                print "Left file: " + view.file_name()
                self.left = view

            elif view.id() == diffView.right.id():
                print "Right file: " + view.file_name()
                self.right = view

            if self.left != None and self.right != None:
                diffView.loadDiff()
                self.left = None
                self.right = None

    def on_pre_save(self, view):
        global diffView

        if (diffView):
            if view.id() == diffView.left.id():
                diffView.abandonUnmergedDiffs('left')

            elif view.id() == diffView.right.id():
                diffView.abandonUnmergedDiffs('right')

    def on_post_save(self, view):
        global diffView

        if diffView and (view.id() == diffView.left.id() or view.id() == diffView.right.id()):
            wnd = view.window()
            if wnd:
                wnd.run_command('close_window')

    def on_close(self, view):
        global diffView

        if diffView != None:
            if view.id() == diffView.left.id():
                wnd = diffView.right.window()
                if wnd != None:
                    wnd.run_command('close_window')
                diffView = None

            elif view.id() == diffView.right.id():
                wnd = diffView.left.window()
                if wnd != None:
                    wnd.run_command('close_window')
                diffView = None

    def on_selection_modified(self, view):
        if diffView:
            diffView.checkForClick(view)
