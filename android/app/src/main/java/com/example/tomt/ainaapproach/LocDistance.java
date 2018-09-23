package com.example.tomt.ainaapproach;

/**
 * Created by Tom on 01.12.17.
 */

public class LocDistance {
    private double distance;
    private String location;

    public LocDistance(double distance, String location) {
        this.distance = distance;
        this.location = location;
    }

    public double getDistance() {
        return distance;
    }

    public String getLocation() {
        return location;
    }


}