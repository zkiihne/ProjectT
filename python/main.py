import numpy as np
import os, pygame, sys, time
import pdb

from pygame.locals import *

# Create Window
resolution = (1125, 2436)
scaling = 0.3
disp_width, disp_height = (int(num * scaling) for num in resolution)
window = pygame.display.set_mode((disp_width, disp_height))
pygame.display.set_caption('Project T')

# Read Card File
class Card:
  def __init__(self, card_data):
    self.name, self.type, self.text, self.health = card_data
    self.img = pygame.image.load(os.path.join('assets', 'art', self.name.lower().replace(' ', '_') + '.jpg'))
    self.img.convert()
    self.rect = self.img.get_rect()
    self.rect.center = np.random.randint(disp_width), np.random.randint(disp_height)
    self.mask = pygame.mask.from_surface(self.img)

    self.offsets = (0,0)
    self.clicked = False

  def draw(self, window):
    window.blit(self.img, self.rect)

  def set_pos(self, loc):
    self.rect.center = loc

  def move(self, deltas):
    self.rect.move_ip(deltas)

cards = []
with open(os.path.join('assets', 'cards.txt'), 'r') as file:
  lines = file.readlines()
  for num, line in enumerate(lines):
    line = line.strip()
    if line == '':
      continue
    if num % 5 == 0:
      cards.append([line])
    else:
      cards[num // 5].append(line)
cards = [Card(card_data) for card_data in cards]

# Game Loop
run = True
fps = 60
clock = pygame.time.Clock()

while run:
  window.fill((0,0,0))
  for card in cards:
    card.draw(window)

  for event in pygame.event.get():
    if event.type == QUIT:
      pygame.quit()
      sys.exit()

    if event.type == KEYDOWN:
      if event.key == K_ESCAPE:
        pygame.quit()
        sys.exit()
      if event.key == K_a:
        for card in cards:
          card.move((-5,0))
      if event.key == K_d:
        for card in cards:
          card.move((5,0))
      if event.key == K_s:
        for card in cards:
          card.move((0,-5))
      if event.key == K_w:
        for card in cards:
          card.move((0,5))

    if event.type == pygame.MOUSEBUTTONDOWN:
      if event.button == 1:
        for card in cards:
          if card.rect.collidepoint(event.pos):
            card.clicked = True

    if event.type == pygame.MOUSEMOTION:
      for card in cards:
        if card.clicked == True:
          card.move(event.rel)

    if event.type == pygame.MOUSEBUTTONUP:
      if event.button == 1:
        for card in cards:
          card.clicked = False

  clock.tick(fps)
  pygame.display.update()

