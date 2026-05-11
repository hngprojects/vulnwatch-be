package com.vulnwatch.worker.scanners.dns.models;

public record IpMetadata(
        String ip,
        int asn,
        String org,
        String country
) {
}
