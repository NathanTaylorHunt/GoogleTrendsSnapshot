import sys
from time import sleep

from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.chrome.options import Options

from PIL import Image

WINDOW_SIZE = "1920,1080"
#CHROMEDRIVER_PATH = './drivers'

chrome_options = Options()
chrome_options.add_argument('--headless')
chrome_options.add_argument('--window-size=%s' % WINDOW_SIZE)
#chrome_options.binary_location = CHROME_PATH

def repl(args=None):
    if args is None:
        args = sys.argv[1:]
    
    take_snapshot(['funhaus', 'achievement hunter'])

def take_snapshot(terms):
    browser = webdriver.Chrome(
        #executable_path=CHROMEDRIVER_PATH,
        chrome_options=chrome_options)
    browser.get('https://trends.google.com/trends/')
    searchbar = browser.find_element_by_tag_name('search').find_element_by_tag_name('input')
    searchbar.send_keys(', '.join(terms))
    searchbar.send_keys(Keys.ENTER)

    sleep(3)

    header = browser.find_element_by_class_name('explorepage-content-header')
    header_loc = header.location
    header_size = header.size

    graph = browser.find_element_by_tag_name('widget')
    graph_loc = graph.location
    graph_size = graph.size

    line_chart = browser.find_element_by_css_selector('line-chart-directive svg')
    hover = ActionChains(browser).move_to_element_with_offset(
            line_chart,
            int(line_chart.size['width'] * 0.98),
            int(line_chart.size['height'] / 2))
    hover.perform()

    browser.save_screenshot('screenshot.png')
    browser.quit()

    img = Image.open('screenshot.png')
    left = graph_loc['x']
    top = header_loc['y']
    right = graph_loc['x'] + graph_size['width']
    bottom = graph_loc['y'] + graph_size['height']

    img = img.crop((left, top, right, bottom))
    img.save('screenshot.png')

if __name__ == '__main__':
    repl()