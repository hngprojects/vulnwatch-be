package com.vulnwatch.worker.scanners.dns.utility;

import com.vulnwatch.worker.scanners.dns.models.Finding;
import com.vulnwatch.worker.scanners.dns.models.ScanContext;
import com.vulnwatch.worker.scanners.dns.rules.Rule;
import java.util.*;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/** Service that applies rules to scan context */
@Service
@RequiredArgsConstructor
public class RuleEngine {

  private final List<Rule> rules;

  public Map<String, Object> scanJob(ScanContext context) {

    Map<String, Object> result = new LinkedHashMap<>();
    List<Finding> findings = new ArrayList<>();

    result.put("aRecords", context.aRecordList());
    result.put("aaaaRecords", context.aaaaRecordList());
    result.put("nsRecords", context.nsRecordList());
    result.put("mxRecords", context.mxRecordList());
    result.put("dsRecords", context.dsRecordList());
    result.put("dnsKeyRecords", context.dnsKeyRecordList());
    result.put("cnameRecords", context.cnameRecordList());
    result.put("txtRecords", context.txtRecordList());
    result.put("ipMetadata", context.ipMetadataList());

    for (Rule rule : rules) {
      findings.addAll(rule.evaluate(context));
    }
    result.put("findings", findings);

    return result;
  }
}
