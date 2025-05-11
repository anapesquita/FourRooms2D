# FourRooms2D

A Unity-based experimental platform for studying spatial navigation, reinforcement learning, and decision-making behaviors.

## Overview

FourRooms2D is a configurable experimental environment featuring a four-room grid layout where participants navigate to find rewards under different experimental conditions. The platform is designed for researchers studying spatial cognition, learning, memory processes, and decision-making.

## Task Variants

The project includes several experimental paradigms:

### nav2D_probablistic
- Implements probabilistic reinforcement learning in spatial navigation
- Features rewards that appear with varying probabilities in different rooms
- Structured in blocks with 16 trials per block, 3 blocks per run
- Measures adaptive learning and probability inference

### nav2D_reversal_2cues
- Focuses on reversal learning with multiple visual cues
- Tests cognitive flexibility through mid-experiment reversals of reward associations
- Uses 32-trial training blocks followed by 16-trial testing blocks
- Measures adaptation to changing reward contingencies

### nav2D_separation
- Examines pattern separation and completion in spatial contexts
- Tests ability to distinguish between similar reward locations
- Uses multiple reward types (pineapple, banana, mushroom, avocado)
- Block structure: 8 trials per reward type, 4 reward types

### micro2D_debug_portal
- Specialized variant implementing teleportation portals between rooms
- Tests path integration and spatial updating with teleportation
- Features fixed portal locations and consistent reward positions
- Measures navigation efficiency and teleportation effects

## Key Features

- Four-room grid environment with configurable navigation parameters
- Multiple reward types and presentation conditions
- Detailed movement tracking and response time measurement
- Customizable experimental parameters (timing, jitter, block structure)
- JSON-based data storage with comprehensive behavioral metrics
- Movement visualizations and replay capabilities

## Data Collection

All task variants record standardized data including:
- Timestamped player coordinates
- State transitions and event markers
- Success/error flags and response times
- Score information and reward collection patterns
- Room transitions and navigation paths

## Requirements

- Unity 2020.3 or newer
- Windows 10/11, macOS, or Linux
- Recommended: 8GB RAM, dedicated graphics
- Optional: Python 3.7+ (for data analysis scripts)

