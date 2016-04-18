import argparse
import json
import logging
import os
import random

logger = logging.getLogger()
logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)-7s - [%(funcName)s] %(message)s')
# uncomment for submission
# logger.disabled = True

ACTIONS = {
	-1: 'DoNothing',
	1: 'MoveUp',
	2: 'MoveLeft',
	3: 'MoveRight',
	4: 'MoveDown',
	5: 'PlaceBomb',
	6: 'TriggerBomb',
}


def main(player_key, output_path):

	logger.info('Player key: {}'.format(player_key))
	logger.info('Output path: {}'.format(output_path))

	with open(os.path.join(output_path, 'state.json'), 'r') as f:
		state = json.load(f)
		logger.info('State: {}'.format(state))

	action = random.choice(list(ACTIONS.keys()))
	logger.info('Action: {}'.format(ACTIONS[action]))

	with open(os.path.join(output_path, 'move.txt'), 'w') as f:
		f.write('{}\n'.format(action))


if __name__ == '__main__':
	parser = argparse.ArgumentParser()
	parser.add_argument('player_key', nargs='?')
	parser.add_argument('output_path', nargs='?', default=os.getcwd())
	args = parser.parse_args()

	assert(os.path.isdir(args.output_path))

	main(args.player_key, args.output_path)
