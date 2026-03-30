#!/bin/bash

echo "Installing SOG dependencies..."

if [ -f /etc/debian_version ]; then
    sudo apt install -y mpg123 dbus-x11
elif [ -f /etc/fedora-release ]; then
    sudo dnf install -y mpg123 dbus-x11
elif [ -f /etc/arch-release ]; then
    sudo pacman -S --noconfirm mpg123 dbus-x11
else
    echo "Distro not recognized. Please install manually: mpg123, dbus-x11"
fi

echo "Done!"