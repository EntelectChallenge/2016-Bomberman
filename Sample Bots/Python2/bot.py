import random
import sys
from os import path

def main(output_path, player_key):
    possible_moves = [
        {'Command': 'DoNothing', 'CommandCode': -1},
        {'Command': 'MoveUp', 'CommandCode': 1},
        {'Command': 'MoveLeft', 'CommandCode': 2},
        {'Command': 'MoveRight', 'CommandCode': 3},
        {'Command': 'MoveDown', 'CommandCode': 4},
        {'Command': 'PlaceBomb', 'CommandCode': 5},
        {'Command': 'TriggerBomb', 'CommandCode': 6}]
    move = random.choice(possible_moves)
    move_file = open(path.join(output_path, 'move.txt'), 'w')
    move_file.write(str(move['CommandCode']) + '\r\n')
    move_file.close()

    state_file = open(path.join(output_path, 'state.json'), 'r')
    state_file.close()

if __name__ == '__main__':
    if(len(sys.argv) < 2):
        player_key = ''
    else:
        player_key = sys.argv[1]

    if(len(sys.argv) < 3):
        output_path = ''
    else:
        output_path = sys.argv[2]

    if (output_path != '' and path.exists(output_path) == False):
       print
       print ('Error: Output folder "' + sys.argv[1] + '" does not exist.')
       exit(-1)

    main(output_path, player_key)
