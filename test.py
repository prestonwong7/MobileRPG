#Testing File, test.py

print("Hey guys, this is a testing file, feel free to change anything")
print("Try using master branches, and changing different branches")
print("Merge")
print("Pull, push, commit!")

print("Test ride")

print("Omae Wa Mou Shindeiru")

print("I am the best at CECS 341")

print("Nerf this")


print("Embrace the Iris")

import numpy as np

def passwords(N, size):
    success = 0
    counter = N
    while counter > 0:
        hackerList = np.random.randint(1, 456976, size)
        userPass = np.random.randint(1,456976)
        if userPass in hackerList:
            success = success + 1
        counter = counter -1
    print ('Probability is', success / N)

passwords(1000, 1000000)

