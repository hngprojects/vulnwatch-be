package com.vulnwatch.worker.processors;

import com.vulnwatch.worker.models.ScanJob;
import com.vulnwatch.worker.scanners.dns.DnsScanner;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * ScanProcessor: Orchestrates which scanners to run for a given job.
 * Intern-friendly: Think of this as the manager who delegates work to specialists.
 */
@Service
@RequiredArgsConstructor
public class ScanProcessor {

    private final DnsScanner dnsScanner;
    // Add other scanners here: SSL, Headers, etc.

    public void process(ScanJob job) {
//        System.out.println("Processing job " + job.getScan_id() + " for domain " + job.getDomain());
//
//        for (String type : job.getScan_type()) {
//            switch (type.toLowerCase()) {
//                case "dns":
//                    dnsScanner.scan(job);
//                    break;
//                case "ssl":
//                    // TODO: Implement SslScanner
//                    System.out.println("SSL scan requested (Not implemented yet)");
//                    break;
//                case "headers":
//                    // TODO: Implement HeaderScanner
//                    System.out.println("Headers scan requested (Not implemented yet)");
//                    break;
//                default:
//                    System.out.println("Unknown scan type: " + type);
//            }
//        }
    }
}
