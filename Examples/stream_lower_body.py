import time

import hardware
import imumocap.solvers
import models

# Load example model
model = models.LowerBody()

# Connect to and configure IMUs
ignored = [
    model.left_toe.name,
    model.right_toe.name,
]  # there are no IMUs on the toes

imus = hardware.setup([l.name for l in model.root.flatten() if l.name not in ignored] )

# Stream to IMU Mocap Viewer
connection = imumocap.viewer.Connection()

north = imumocap.solvers.North()

def calibrate() -> None:
    print("Calibrating in...")

    for countdown in [3, 2, 1]:
        print(countdown)

        time.sleep(0.5)

    north.set(imus[model.root.name].matrix)
    
    imumocap.solvers.calibrate(model.root, {n: i.matrix for n, i in imus.items()})

    print("Calibrated")


while True:
    time.sleep(1 / 30)  # 30 fps

    if any([i.button_pressed for i in imus.values()]):
        calibrate()

    imu_globals = north.apply({n: i.matrix for n, i in imus.items()})

    for name, matrix in imu_globals.items():
        model.root.dictionary()[name].set_joint_from_imu_global(matrix)

    imumocap.solvers.floor(model.root)

    connection.send(imumocap.viewer.link_to_primitives(model.root))
