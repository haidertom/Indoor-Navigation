package com.example.tomt.ainaapproach;

/**
 * Created by Tom on 01.12.17.
 */

public class LogRecord {

    private String bssid;
    private int rss;

    public LogRecord(String bssid, int rss) {
        super();
        this.bssid = bssid;
        this.rss = rss;
    }

    public String getBssid() {
        return bssid;
    }

    public int getRss() {
        return rss;
    }

    public String toString() {
        String str = new String();
        str = String.valueOf(bssid) + " " + String.valueOf(rss) + "\n";
        return str;
    }

}
