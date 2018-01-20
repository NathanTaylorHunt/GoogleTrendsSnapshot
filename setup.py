
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