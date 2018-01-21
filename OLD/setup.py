'''
from setuptools import setup

setup(
    name='google_trends_snapshot',
    version='0.1.0',
    packages=['google_trends_snapshot'],
    entry_points={
        'console_scripts': [
            'google_trends_snapshot=google_trends_snapshot.__main__:main'
        ]
    }
)
'''

import sys
from cx_Freeze import setup, Executable

build_exe_options = {'packages': ['os']}

setup(
    name='google_trends_snapshot',
    version='0.1',
    description='Automated script to take snapshots of Google Trends graph.',
    options={'build_exe': build_exe_options},
    executables=[Executable('run.py', base=None)]
)