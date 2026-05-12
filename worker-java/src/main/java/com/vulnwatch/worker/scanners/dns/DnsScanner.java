package com.vulnwatch.worker.scanners.dns;

import com.vulnwatch.worker.models.ScanJob;
import org.springframework.stereotype.Service;
import org.xbill.DNS.*;
import org.xbill.DNS.Record;

@Service
public class DnsScanner {
    
    /**
     * Performs a DNS scan for the given domain.
     * Beginners: This is where we check for MX, TXT, and A records.
     */


    public void scan(ScanJob job) {
        System.out.println("Starting DNS scan for domain: " + job.getDomainName());

        String domain = job.getDomainName();


    }


}
