import sys
import os
from time import sleep

from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.chrome.options import Options

from PIL import Image

PROMPT = '> '
AUTO_CAP = True
WINDOW_SIZE = "1920,1080"
#CHROMEDRIVER_PATH = './drivers'

chrome_options = Options()
chrome_options.add_argument('--headless')
chrome_options.add_argument('--window-size=%s' % WINDOW_SIZE)
#chrome_options.binary_location = CHROME_PATH

def repl():
    cls()
    print('\nWELCOME TO GOOGLE TRENDS SNAPSHOT')

    should_quit = False
    while not should_quit:
        print('\n\nEnter round term:')
        round_term = str(input(PROMPT))
        if AUTO_CAP:
            round_term = round_term.upper()

        if round_term == 'exit':
            should_quit = True
            return

        delay_clear = False
        entering_terms = True
        search_terms = []
        while entering_terms:
            if not delay_clear:
                cls()
            else:
                delay_clear = False
            print('\n')
            print('ROUND: ' + round_term)
            if search_terms:
                print('\nSEARCH TERMS:')
                for idx, term in enumerate(search_terms):
                    print('(%d) \'%s\'' % (idx+1, term))
            else:
                print('\nSEARCH TERMS: NONE')

            print('\nEnter a new search term (5 max).')
            print('Type \'snap\' to take a snapshot.')
            print('Type \'delete\' plus a number to delete a term.')
            print('Type \'clear\' to clear search terms.')
            print('Type \'new\' to start a new round.')
            print('Type \'exit\' to quit.')
            print('')

            search_term = str(input(PROMPT))

            if search_term == 'exit':
                entering_terms = False
                should_quit = True

            if search_term == 'snap':
                if len(search_terms) > 0:
                    take_snapshot(round_term, search_terms)
                    delay_clear = True
            elif search_term.split(' ')[0] == 'delete':
                if len(search_term.split(' ')) > 1:
                    term_idx = int(search_term.split(' ')[1])
                    if term_idx > 0 and term_idx <= len(search_terms):
                        del search_terms[term_idx-1]
            elif search_term == 'clear':
                search_terms = []
            elif search_term == 'new':
                search_terms = []
                entering_terms = False
            else:
                if len(search_terms) < 5:
                    if AUTO_CAP:
                        search_term = search_term.upper()
                    search_terms.append(search_term)
        cls()
    
    print('\n\nGoodbye :)')

def take_snapshot(round_term, search_terms):
    print('\n\nTAKING SNAPSHOT, PLEASE WAIT...')

    browser = webdriver.Chrome(
        #executable_path=CHROMEDRIVER_PATH,
        chrome_options=chrome_options)
    browser.get('https://trends.google.com/trends/')
    searchbar = browser.find_element_by_tag_name('search').find_element_by_tag_name('input')
    searchbar.send_keys(', '.join(search_terms))
    searchbar.send_keys(Keys.ENTER)

    sleep(3)

    header = browser.find_element_by_class_name('explorepage-content-header')
    header_loc = header.location

    graph = browser.find_element_by_tag_name('widget')
    graph_loc = graph.location
    graph_size = graph.size

    line_chart = browser.find_element_by_css_selector('line-chart-directive svg')
    hover = ActionChains(browser).move_to_element_with_offset(
            line_chart,
            int(line_chart.size['width'] * 0.98),
            int(line_chart.size['height'] / 2))
    hover.perform()

    snapshot_dir = './snapshots'
    num_files = len(os.listdir(snapshot_dir))
    file_name = '%s/%03d-snapshot-%s.png' % (snapshot_dir, num_files, round_term)

    browser.save_screenshot(file_name)
    browser.quit()

    img = Image.open(file_name)
    left = graph_loc['x']
    top = header_loc['y']
    right = graph_loc['x'] + graph_size['width']
    bottom = graph_loc['y'] + graph_size['height']

    img = img.crop((left, top, right, bottom))
    img.save(file_name)

    final_img = Image.open(file_name)
    final_img.show()

    print('\n\nSNAPSHOT COMPLETE -- saved as ' + file_name)
    print('\n\n')

def cls():
    os.system('cls' if os.name=='nt' else 'clear')

if __name__ == '__main__':
    repl()