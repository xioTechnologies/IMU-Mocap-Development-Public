from typing import Dict

import numpy as np

from ..matrix import Matrix


class North:
    def __init__(self) -> None:
        self.__matrix = Matrix()

    def set(self, imu_global: Matrix) -> None:
        self.__matrix = get_heading(imu_global, True)

    def apply(self, imu_globals: Dict[str, Matrix]) -> Dict[str, Matrix]:  # {<link name>: <IMU measurment>, ...}
        return {n: self.__matrix * m for n, m in imu_globals.items()}


@staticmethod
def project_to_ground(vector: np.ndarray) -> np.ndarray:
    projected = np.array([vector[0, 0], 0, vector[0, 2]])
    return projected / np.linalg.norm(projected)

@staticmethod
def calculate_heading_matrix(projected_vector: np.ndarray) -> Matrix:
    angle = np.arctan2(projected_vector[0], projected_vector[2])
    return Matrix(rot_y=np.degrees(angle))

@staticmethod
def get_heading(matrix: Matrix, upright: bool) -> Matrix:
    
    axis_index = 2 if upright else 0
    
    forward_vector = matrix.rotation[:, axis_index].flatten()

    ground_vector = project_to_ground(forward_vector)
    
    heading_matrix = calculate_heading_matrix(ground_vector)

    return heading_matrix
