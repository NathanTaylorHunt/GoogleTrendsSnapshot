import sys
from time import sleep

from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.action_chains import ActionChains

from PIL import Image

trends = []


def repl(args=None):
    if args is None:
        args = sys.argv[1:]
    
    launch_browser()

def launch_browser():
    browser = webdriver.Chrome()
    browser.get('https://trends.google.com/trends/')
    searchbar = browser.find_element_by_tag_name('search').find_element_by_tag_name('input')
    searchbar.send_keys('funhaus, rooster teeth')
    searchbar.send_keys(Keys.ENTER)

    sleep(3)

    header = browser.find_element_by_class_name('explorepage-content-header')
    header_loc = header.location
    header_size = header.size

    graph = browser.find_element_by_tag_name('widget')
    graph_loc = graph.location
    graph_size = graph.size

    line_chart = browser.find_element_by_tag_name('line-chart-directive')
    hover = ActionChains(browser).move_to_element_with_offset(
            line_chart,
            int(line_chart.size['width'] * 0.98),
            int(line_chart.size['height'] / 2))
    hover.perform()

    browser.save_screenshot('screenshot.png')
    browser.quit()

    img = Image.open('screenshot.png')
    left = header_loc['x']
    top = header_loc['y']
    right = header_loc['x'] + header_size['width']
    bottom = graph_loc['y'] + graph_size['height']

    img = img.crop((left, top, right, bottom))
    img.save('screenshot2.png')

if __name__ == '__main__':
    repl()