package com.vulnwatch.worker.scanners.dns.models;

import lombok.Builder;
import lombok.NoArgsConstructor;
import org.hibernate.annotations.processing.Find;
import org.xbill.DNS.*;

import java.util.List;
import java.util.Map;

@Builder
public record DnsScanResult(
        Map<String, Object> scanResults
) {

}
